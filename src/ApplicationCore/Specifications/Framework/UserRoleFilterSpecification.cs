using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class UserRoleFilterSpecification : Specification<UserRole>
	{
		public UserRoleFilterSpecification(int id, bool withChild = true)
		{
			InitializeFilterData(id: id, withChild: withChild);
		}

		public UserRoleFilterSpecification(int skip, int take, bool withChild = false)
		{
			InitializeFilterData(skip, take, withChild: withChild);
		}

		public UserRoleFilterSpecification(string userId, string userName, bool withChild = false)
		{
			InitializeFilterData(userId: userId, userName: userName, withChild: withChild);
		}

		public UserRoleFilterSpecification(int skip, int take, string userId, string userName, bool withChild = false)
		{
			InitializeFilterData(skip, take, userId: userId, userName: userName, withChild: withChild);
		}

		private void InitializeFilterData(int? skip = null, int? take = null, int? id = null, string userId = null, string userName = "", bool? withChild = null)
		{
			Query
				.Where(
					e => (string.IsNullOrEmpty(userId) || e.UserInfoId == userId) &&
					(string.IsNullOrEmpty(userName) || e.UserInfo.UserName.Contains(userName)) &&
					(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withChild.HasValue && withChild.Value)
				Query
					.Include(p => p.UserRoleDetails)
					.ThenInclude(d => d.Role);
		}
	}
}
