using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface IJobConfigurationRepository : IAsyncRepository<JobConfiguration>
	{
		#region appgen: repository method
		Task<JobConfiguration> CloneEntity(int id, string userName);

		#endregion
	}
}
