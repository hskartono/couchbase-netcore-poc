using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface ISchedulerCronIntervalRepository : IAsyncRepository<SchedulerCronInterval>
	{
		#region appgen: repository method
		Task<SchedulerCronInterval> CloneEntity(string id, string userName);

		#endregion
	}
}
