using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using System.Collections.Generic;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class UserInfoFilterSpecification : Specification<UserInfo>
	{
		public UserInfoFilterSpecification(string userName)
		{
			InitializeFilterData(userName: userName);
		}

		public UserInfoFilterSpecification(List<string> userNames)
		{
			//foreach (var user in userNames)
			//	Query.Where(e => userNames.Contains(user));
			Query.Where(e => userNames.Contains(e.UserName));
		}

		public UserInfoFilterSpecification(int skip, int take)
		{
			InitializeFilterData(skip, take);
		}

		public UserInfoFilterSpecification(string userName, string firstName, string lastName)
		{
			InitializeFilterData(userName: userName, firstName: firstName, lastName: lastName);
		}

		public UserInfoFilterSpecification(int skip, int take, string userName, string firstName, string lastName)
		{
			InitializeFilterData(skip, take, userName, firstName, lastName);
		}

		private void InitializeFilterData(int? skip = null, int? take = null, string userName = "", string firstName = "", string lastName = "")
		{

			if (!string.IsNullOrEmpty(userName))
				Query.Where(e => e.UserName.Contains(userName));

			if (!string.IsNullOrEmpty(firstName))
				Query.Where(e => e.FirstName.Contains(firstName));

			if (!string.IsNullOrEmpty(lastName))
				Query.Where(e => e.LastName.Contains(lastName));

			if (take.HasValue && take.Value > 0)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
