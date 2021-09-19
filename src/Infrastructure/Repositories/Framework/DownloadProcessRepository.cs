using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class DownloadProcessRepository : AsyncRepository<DownloadProcess>, IDownloadProcessRepository
	{
		public DownloadProcessRepository(AppDbContext context) : base(context) { }
	}
}
