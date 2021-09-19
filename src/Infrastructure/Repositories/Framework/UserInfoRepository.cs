using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class UserInfoRepository:AsyncRepository<UserInfo>, IUserInfoRepository
	{
		public UserInfoRepository(AppDbContext context) : base(context) { }
	}
}
