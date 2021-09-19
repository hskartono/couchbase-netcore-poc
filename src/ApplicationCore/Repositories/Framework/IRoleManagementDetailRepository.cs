using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface IRoleManagementDetailRepository : IAsyncRepository<RoleManagementDetail>
	{
		#region appgen: repository method
		Task<RoleManagementDetail> CloneEntity(int id, string userName);

		#endregion
	}
}
