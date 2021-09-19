using AppCoreApi.PublicApi.Features.FunctionInfos;
using AppCoreApi.PublicApi.Features.ModuleInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Roles
{
	public class RoleDetailDTO
	{
		public int Id { get; set; }
		public int RoleId { get; set; }
		public string FunctionInfoId { get; set; }
		public string FunctionName { get; set; }
		public FunctionInfoDTO FunctionInfo { get; set; }
		public string ModuleName { get; set; }
		//public ModuleInfoDTO ModuleInfo { get; set; }
		public bool AllowCreate { get; set; }
		public bool AllowRead { get; set; }
		public bool AllowUpdate { get; set; }
		public bool AllowDelete { get; set; }
		public bool ShowInMenu { get; set; }
		public bool AllowDownload { get; set; }
		public bool AllowPrint { get; set; }
		public bool AllowUpload { get; set; }
		public int orderPosition { get; set; }

	}
}
