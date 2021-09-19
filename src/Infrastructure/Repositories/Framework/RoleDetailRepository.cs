using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class RoleDetailRepository : AsyncRepository<RoleDetail>, IRoleDetailRepository
	{
		public RoleDetailRepository(AppDbContext context) : base(context) { }
	}
}
