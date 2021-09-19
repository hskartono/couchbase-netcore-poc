using AppCoreApi.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCoreApi.PublicApi.Features.FunctionInfos;


namespace AppCoreApi.PublicApi.Features.RoleManagements
{
	public class RoleManagementDetailDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string RoleManagementId { get; set; }
		public string RoleManagementName { get; set; }
		//public RoleManagementDTO RoleManagement { get; set; }
		public string FunctionInfoId { get; set; }
		public string FunctionInfoName { get; set; }
		//public FunctionInfoDTO FunctionInfo { get; set; }
		public bool? AllowCreate { get; set; }
		public bool? AllowRead { get; set; }
		public bool? AllowUpdate { get; set; }
		public bool? AllowDelete { get; set; }
		public bool? ShowInMenu { get; set; }
		public bool? AllowDownload { get; set; }
		public bool? AllowPrint { get; set; }
		public bool? AllowUpload { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
