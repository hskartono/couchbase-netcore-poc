using AppCoreApi.ApplicationCore.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore
{
	public class AppSettings : IAppSettings
	{
		private readonly LookupSettings _lookupSettings;
		private readonly PathConfig _pathConfig;
		private readonly EmailConfiguration _emailConfig;
		private readonly Passport _passport;

		public AppSettings(LookupSettings lookupSettings)
		{
			_lookupSettings = lookupSettings;
			_pathConfig = _lookupSettings.PathConfig;
			_emailConfig = _lookupSettings.EmailConfiguration;
			_passport = _lookupSettings.Passport;
		}

        public PathConfig PathConfig => _pathConfig;

        public EmailConfiguration Smtp => _emailConfig;

		public Passport Passport => _passport;
    }
}
