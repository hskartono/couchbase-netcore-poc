using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AppCoreApi.PublicApi.Features.RoleManagements
{
	public class RoleManagementDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<RoleManagementDetailDTO> RoleManagementDetail { get; set; } = new List<RoleManagementDetailDTO>();

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
