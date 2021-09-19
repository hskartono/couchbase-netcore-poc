using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Entities
{
	public class Role : BaseEntity
	{
		private List<RoleDetail> _roleDetails = new List<RoleDetail>();
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public IReadOnlyList<RoleDetail> RoleDetails => _roleDetails;

		public void AddOrUpdateRoleDetail(RoleDetail roleDetail)
		{
			if (roleDetail == null)
				throw new ArgumentNullException(nameof(roleDetail));

			var data = _roleDetails.SingleOrDefault(o => o.FunctionInfoId == roleDetail.FunctionInfoId);
			if(data == null)
			{
				roleDetail.Role = this;
				roleDetail.RoleId = Id;
				_roleDetails.Add(roleDetail);
			} else
			{
				data.FunctionInfo = roleDetail.FunctionInfo;
				data.FunctionInfoId = roleDetail.FunctionInfoId;
				data.AllowCreate = roleDetail.AllowCreate;
				data.AllowRead = roleDetail.AllowRead;
				data.AllowUpdate = roleDetail.AllowUpdate;
				data.AllowDelete = roleDetail.AllowDelete;
				data.ShowInMenu = roleDetail.ShowInMenu;
				data.AllowDownload = roleDetail.AllowDownload;
				data.AllowPrint = roleDetail.AllowPrint;
			}
		}

		public void AddOrUpdateRoleDetail(FunctionInfo functionInfo, 
			bool allowCreate, bool allowRead, bool allowUpdate, bool allowDelete, 
			bool allowDownload, bool allowPrint, bool showInMenu, bool allowUpload)
		{
			if (functionInfo == null)
				throw new ArgumentNullException(nameof(functionInfo));

			var data = _roleDetails.SingleOrDefault(o => o.FunctionInfoId == functionInfo.Id);
			if (data == null)
			{
				data = new RoleDetail(this, functionInfo, allowCreate, allowRead, allowUpdate, allowDelete, allowDownload, allowPrint, showInMenu, allowUpload);
				_roleDetails.Add(data);
			} else
			{
				data.FunctionInfo = functionInfo;
				data.AllowCreate = allowCreate;
				data.AllowRead = allowRead;
				data.AllowUpdate = allowUpdate;
				data.AllowDelete = allowDelete;
				data.ShowInMenu = showInMenu;
				data.AllowDownload = allowDownload;
				data.AllowPrint = allowPrint;
				data.AllowUpload = allowUpload;
			}
		}

		public void RemoveRoleDetail(FunctionInfo functionInfo)
		{
			if (functionInfo == null)
				throw new ArgumentNullException(nameof(functionInfo));

			var data = _roleDetails.SingleOrDefault(o => o.FunctionInfoId == functionInfo.Id);
			if (data != null) _roleDetails.Remove(data);
		}

		public void RemoveRoleDetail(RoleDetail roleDetail)
		{
			if (_roleDetails.Contains(roleDetail))
				_roleDetails.Remove(roleDetail);
		}

		public void ClearRoleDetail()
		{
			_roleDetails.Clear();
		}
		
		//Tambahan
		public void SortRoleDetails()
		{

			//_roleDetails = _roleDetails.OrderBy(e => e.FunctionInfo?.ModuleInfo?.OrderPosition).ThenBy(e => e.FunctionInfo?.Name).ToList();
			_roleDetails = _roleDetails.OrderBy(e => e.ModuleOrder).ThenBy(e => e.FunctionName).ToList();

			int index = 0;
			foreach(var item in _roleDetails)
            {
				item.OrderPosition = index;
				index++;
            }
			//this.ClearRoleDetail();
			// _roleDetails.AddRange(detail);
		}
	}
}
