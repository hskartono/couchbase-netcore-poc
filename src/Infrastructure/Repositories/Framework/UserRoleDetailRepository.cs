using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class UserRoleDetailRepository : AsyncRepository<UserRoleDetail>, IUserRoleDetailRepository
	{
		public UserRoleDetailRepository(AppDbContext context) : base(context) { }
	}
}
