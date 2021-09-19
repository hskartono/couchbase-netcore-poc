using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IFunctionInfoRepository : IAsyncRepository<FunctionInfo>
	{
		Task<bool> searcFungsionInfos(string id);
	}
}
