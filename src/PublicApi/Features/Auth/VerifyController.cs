using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        [HttpPost]
        public IActionResult Verify([FromBody] string token, CancellationToken cancel = default)
        {
            //return Ok("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhdXRoMHw2MDA2OTg2YWY2ZmFjNjAwNmE2MWY0ZGQiLCJ1bmlxdWVfbmFtZSI6ImF1dGgwfDYwMDY5ODZhZjZmYWM2MDA2YTYxZjRkZCIsImV4cCI6MTY1NTM1MTM3OSwiaXNzIjoiVEFNLlNTTyIsImF1ZCI6IjA5MDEzNmEzLTQ4NjktNGFiMi1mMDMyLTA4ZDkyNDAxYTU5YyJ9.aKFuyyyYAlxbTfCc395EOubI4mIl3M_PSmdKqH1f51Q");
            return Ok(token);
        }
    }
}
