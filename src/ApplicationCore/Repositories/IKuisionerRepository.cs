using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Specifications.Filters;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IKuisionerRepository : IAsyncRepository<Kuisioner>
	{
		#region appgen: repository method
		Task<Kuisioner> CloneEntity(int id, string userName);

		#endregion
	}
}
