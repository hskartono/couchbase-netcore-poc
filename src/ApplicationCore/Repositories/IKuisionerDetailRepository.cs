using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Specifications.Filters;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IKuisionerDetailRepository : IAsyncRepository<KuisionerDetail>
	{
		#region appgen: repository method
		Task<KuisionerDetail> CloneEntity(int id, string userName);

		#endregion
	}
}
