using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface ISchedulerConfigurationRepository : IAsyncRepository<SchedulerConfiguration>
	{
		#region appgen: repository method
		Task<SchedulerConfiguration> CloneEntity(int id, string userName);

		#endregion
	}
}
