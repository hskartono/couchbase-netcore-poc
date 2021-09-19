using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi
{
	public class TokenJsonObject
	{
		public string Iss { get; set; }
		public string Aud { get; set; }
		public string Sub { get; set; }
		public long Iat { get; set; }
		public string IssuedAt { get; set; }
		public long Exp { get; set; }
		public string Expiration { get; set; }
		public string Jti { get; set; }
		public string Unique_name { get; set; }
		public string Email { get; set; }
		public string EmployeeId { get; set; }

	}
}
