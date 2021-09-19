using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class UserRoleDetail:BaseEntity
	{

		public int Id { get; set; }
		public int UserRoleId { get; set; }
		public virtual UserRole UserRole { get; set; }
		public int RoleId { get; set; }
		public virtual Role Role { get; set; }

		public UserRoleDetail()
		{

		}

		public UserRoleDetail(UserRole userRole, Role role)
		{
			UserRole = userRole;
			UserRoleId = userRole.Id;
			Role = role;
			RoleId = role.Id;
		}
	}
}
