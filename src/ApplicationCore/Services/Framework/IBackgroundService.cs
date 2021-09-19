using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IBackgroundService
	{
		Task ExecuteService();
		Task ExecuteService(params object[] list);
		Task ExecuteStoredProcedure(string spName);
	}
}
