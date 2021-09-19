using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IModuleInfoRepository : IAsyncRepository<ModuleInfo>
	{
		#region appgen: repository method
		Task<ModuleInfo> CloneEntity(int id, string userName);

		//TAMBAHAN REZA
		Task<bool> searcFungsionInfos(int id);
		#endregion
	}
}
