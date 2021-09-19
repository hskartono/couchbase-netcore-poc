using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi
{
	public class NoSecurityAuthorizationFilter : IDashboardAuthorizationFilter
	{
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
