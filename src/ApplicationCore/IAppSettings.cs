using AppCoreApi.ApplicationCore.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore
{
	public interface IAppSettings
	{
        public PathConfig PathConfig { get; }
        public EmailConfiguration Smtp { get; }
        public Passport Passport { get; }
    }
}
