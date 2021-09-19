using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class LookupRepository : AsyncRepository<Lookup>, ILookupRepository
	{
		public LookupRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
