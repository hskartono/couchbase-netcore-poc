using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppCoreApi.ApplicationCore.Entities
{
	public class RoleManagementDetail : BaseEntity
	{
		#region appgen: generated constructor
		public RoleManagementDetail() { }

		public RoleManagementDetail(string functionInfoId, bool? allowCreate, bool? allowRead, bool? allowUpdate, bool? allowDelete, bool? showInMenu, bool? allowDownload, bool? allowPrint, bool? allowUpload, RoleManagement parent)
		{
			FunctionInfoId = functionInfoId;
			AllowCreate = allowCreate;
			AllowRead = allowRead;
			AllowUpdate = allowUpdate;
			AllowDelete = allowDelete;
			ShowInMenu = showInMenu;
			AllowDownload = allowDownload;
			AllowPrint = allowPrint;
			AllowUpload = allowUpload;
			RoleManagement = parent;
			if (parent != null) RoleManagementId = parent.Id;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }

		[Column("RoleId")]
		public int? RoleManagementId { get; set; }
		public virtual RoleManagement RoleManagement { get; set; }
		public string FunctionInfoId { get; set; }
		public virtual FunctionInfo FunctionInfo { get; set; }
		public bool? AllowCreate { get; set; }
		public bool? AllowRead { get; set; }
		public bool? AllowUpdate { get; set; }
		public bool? AllowDelete { get; set; }
		public bool? ShowInMenu { get; set; }
		public bool? AllowDownload { get; set; }
		public bool? AllowPrint { get; set; }
		public bool? AllowUpload { get; set; }

		public new int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
