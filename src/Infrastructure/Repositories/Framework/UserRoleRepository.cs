using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class UserRoleRepository : AsyncRepository<UserRole>, IUserRoleRepository
	{
		public UserRoleRepository(AppDbContext context) : base(context) { }
	}
}
