using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore
{
	public class EmailConfiguration
	{
		public string SmtpAddress { get; set; }
		public int PortNumber { get; set; }
		public bool UseSSL { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string SenderName { get; set; }
		public string SenderAddress { get; set; }
		public int BatchLimit { get; set; }

	}
}
