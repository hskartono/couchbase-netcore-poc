using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Auth
{
	public class AuthRequest
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Token { get; set; }

	}
}
