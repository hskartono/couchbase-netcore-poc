using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.UserRoles
{
	public class UserRoleDTO
	{
		public int? Id { get; set; }
		public string UserInfoId { get; set; }
		public List<UserRoleDetailDTO> UserRoleDetails { get; set; } = new List<UserRoleDetailDTO>();

	}
}
