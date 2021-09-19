using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface IRoleManagementRepository : IAsyncRepository<RoleManagement>
	{
		#region appgen: repository method
		Task<RoleManagement> CloneEntity(int id, string userName);

		#endregion
	}
}
