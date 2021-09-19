using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class UserRole : BaseEntity
	{
		private readonly List<UserRoleDetail> _userRoleDetails = new List<UserRoleDetail>();

		public int Id { get; set; }
		public string UserInfoId { get; set; }
		public virtual UserInfo UserInfo { get; set; }
		public IReadOnlyList<UserRoleDetail> UserRoleDetails => _userRoleDetails;

		public void AddRole(Role role)
		{
			if (role == null) return;
			var data = _userRoleDetails.SingleOrDefault(o => o.RoleId == role.Id);
			if (data == null)
				_userRoleDetails.Add(new UserRoleDetail(this, role));
		}

		public void AddOrReplaceRole(UserRoleDetail userRoleDetail)
		{
			if (userRoleDetail == null) return;
			var data = _userRoleDetails.SingleOrDefault(o => o.Id == userRoleDetail.Id);
			if(data == null)
			{
				userRoleDetail.UserRole = this;
				userRoleDetail.UserRoleId = Id;
				userRoleDetail.CompanyId = CompanyId;
				_userRoleDetails.Add(userRoleDetail);
			} else
			{
				data.Role = userRoleDetail.Role;
				data.RoleId = userRoleDetail.RoleId;
			}
		}

		public void RemoveRole(Role role)
		{
			if (role == null) return;
			var data = _userRoleDetails.SingleOrDefault(o => o.RoleId == role.Id);
			if (data != null)
				_userRoleDetails.Remove(data);
		}

		public void RemoveRole(UserRoleDetail userRoleDetail)
		{
			if (userRoleDetail == null) return;
			var data = _userRoleDetails.SingleOrDefault(o => o.Id == userRoleDetail.Id);
			if (data != null)
				_userRoleDetails.Remove(data);
		}

		public void ClearRole()
		{
			_userRoleDetails.Clear();
		}
	}
}
