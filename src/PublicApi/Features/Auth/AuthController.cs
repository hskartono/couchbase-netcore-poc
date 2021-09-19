using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AppCoreApi.ApplicationCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Auth
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class AuthController : BaseAPIController
	{
		private readonly IAppSettings _appSettings;
		public AuthController(IAppSettings appSettings)
		{
			_appSettings = appSettings;
		}

		[HttpPost]
		[Route("Token")]
		public async Task<IActionResult> Token([FromBody] AuthRequest authRequest, CancellationToken cancel = default)
		{
			if(authRequest == null)
			{
				ModelState.AddModelError("AuthRequest", "Informasi otentifikasi harus diisi");
				return ValidationProblem();
			}

			if (string.IsNullOrEmpty(authRequest.Token))
			{
				if(string.IsNullOrEmpty(authRequest.UserName) || string.IsNullOrEmpty(authRequest.Password))
				{
					ModelState.AddModelError("AuthRequest", "Username & password harus diisi");
					return ValidationProblem();
				}
				// username / password mode
			} else
			{
				// token mode
				try
				{
					// validate ke server TAM
					var apiUrl = _appSettings.Passport.VerificationUrl;
					HttpClientHandler clientHandler = new();
					clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

					HttpClient client = new(clientHandler);
					HttpResponseMessage response;
					try
					{
						response = await client.PostAsJsonAsync(apiUrl, authRequest.Token, cancel);
						if (!response.IsSuccessStatusCode)
						{
                            ModelState.AddModelError("verification", response.ReasonPhrase);
                            //return ValidationProblem();
                            return Unauthorized();
						}
					} catch(Exception exItem)
					{
                        ModelState.AddModelError("Verification", exItem.Message);
                        //return ValidationProblem();
                        return Unauthorized();
					}

					TokenJsonObject jsonObj;
					try
					{
						var jsonString = await response.Content.ReadAsStringAsync(cancel);
						jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenJsonObject>(jsonString);
					} catch(Exception exItem)
					{
                        ModelState.AddModelError("verificationResponse", exItem.Message);
                        //return ValidationProblem();
                        return Unauthorized();
					}

					// generate token
					var issuer = jsonObj.Iss;
					var aud = jsonObj.Aud;
					List<Claim> claims;
					try
					{
						claims = new List<Claim>();
						if (!string.IsNullOrEmpty(jsonObj.Sub)) claims.Add(new Claim("sub", jsonObj.Sub));
						if (!string.IsNullOrEmpty(jsonObj.Jti)) claims.Add(new Claim("jti", jsonObj.Jti));
						if (!string.IsNullOrEmpty(jsonObj.Unique_name)) claims.Add(new Claim("unique_name", jsonObj.Unique_name));
						if (!string.IsNullOrEmpty(jsonObj.Email)) claims.Add(new Claim("unique_name", jsonObj.Email));
						if (!string.IsNullOrEmpty(jsonObj.EmployeeId)) claims.Add(new Claim("unique_name", jsonObj.EmployeeId));
						claims.Add(new Claim("iat", jsonObj.Iat.ToString()));
						claims.Add(new Claim("expiration", jsonObj.Exp.ToString()));
					} catch(Exception exItem)
					{
						ModelState.AddModelError("assignClaim", exItem.Message);
						return ValidationProblem();
					}

					if (aud != _appSettings.Passport.ClientId)
					{
                        ModelState.AddModelError("clientid", aud + " vs " + _appSettings.Passport.ClientId);
                        //return ValidationProblem();
                        return Unauthorized();
					}

					SymmetricSecurityKey authSigningKey;
					try
					{
						authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Passport.ClientSecret));
					} catch(Exception exItem)
					{
                        ModelState.AddModelError("generateAuthKey", exItem.Message);
                        //return ValidationProblem();
                        return Unauthorized();
					}

					JwtSecurityToken newToken;
					try
					{
						newToken = new JwtSecurityToken(
						issuer: issuer,
						audience: aud,
						expires: DateTime.Now.AddHours(3),
						claims: claims,
						signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
						);
					} catch(Exception exItem)
					{
                        ModelState.AddModelError("generateNewToken", exItem.Message);
                        //return ValidationProblem();
                        return Unauthorized();
					}

					try
					{
						return Ok(new
						{
							token = new JwtSecurityTokenHandler().WriteToken(newToken),
							expiration = newToken.ValidTo
						});
					} catch(Exception exItem)
					{
                        ModelState.AddModelError("writeToken", exItem.Message);
                        //return ValidationProblem();
                        return Unauthorized();
					}
					
				} catch(Exception ex)
				{
                    ModelState.AddModelError("exception", ex.Message);
                    //return ValidationProblem();
                    return Unauthorized();
				}
			}

            ModelState.AddModelError("other", "not processed");
            //return ValidationProblem();
            return Unauthorized();
		}
	}
}
