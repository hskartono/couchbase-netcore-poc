using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class RoleDetail : BaseEntity
	{
		public int Id { get; set; }
		public int RoleId { get; set; }
		public virtual Role Role { get; set; }
		public string FunctionInfoId { get; set; }
		public virtual FunctionInfo FunctionInfo { get; set; }

		public bool AllowCreate { get; set; }
		public bool AllowRead { get; set; }
		public bool AllowUpdate { get; set; }
		public bool AllowDelete { get; set; }
		public bool ShowInMenu { get; set; }
		public bool AllowDownload { get; set; }
		public bool AllowPrint { get; set; }
		public bool AllowUpload { get; set; }
		[NotMapped]
        public int OrderPosition { get; set; }

		[NotMapped]
        public int ModuleOrder { get; set; }
		[NotMapped]
        public string FunctionName { get; set; }

        public RoleDetail()
		{

		}

		public RoleDetail(Role role, FunctionInfo functionInfo,
			bool allowCreate, bool allowRead, bool allowUpdate, bool allowDelete,
			bool allowDownload, bool allowPrint, bool showInMenu, bool allowUpload)
		{
			Role = role;
			FunctionInfo = functionInfo;
			if (functionInfo != null)
				FunctionInfoId = functionInfo.Id;
			AllowCreate = allowCreate;
			AllowRead = allowRead;
			AllowUpdate = allowUpdate;
			AllowDelete = allowDelete;
			ShowInMenu = showInMenu;
			AllowDownload = allowDownload;
			AllowPrint = allowPrint;
			AllowUpload = allowUpload;
		}

		public RoleDetail(Role role, string functionInfoId,
			bool allowCreate, bool allowRead, bool allowUpdate, bool allowDelete,
			bool allowDownload, bool allowPrint, bool showInMenu, bool allowUpload)
		{
			Role = role;
			FunctionInfoId = functionInfoId;
			AllowCreate = allowCreate;
			AllowRead = allowRead;
			AllowUpdate = allowUpdate;
			AllowDelete = allowDelete;
			ShowInMenu = showInMenu;
			AllowDownload = allowDownload;
			AllowPrint = allowPrint;
			AllowUpload = allowUpload;
		}
	}
}
