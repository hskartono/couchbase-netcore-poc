using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Roles
{
	public class RoleDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public List<RoleDetailDTO> RoleDetails { get; set; } = new List<RoleDetailDTO>();

	}
}
