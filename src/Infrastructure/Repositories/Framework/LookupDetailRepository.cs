using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class LookupDetailRepository : AsyncRepository<LookupDetail>, ILookupDetailRepository
	{
		public LookupDetailRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
