using AppCoreApi.ApplicationCore.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore
{
	public class LookupSettings
	{
        public PathConfig PathConfig { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
        public Passport Passport { get; set; }
    }
}
