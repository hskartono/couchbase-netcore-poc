using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Specifications
{
    public class RoleManagementDetailFilterSpecification : Specification<RoleManagementDetail>
	{
#pragma warning disable IDE0052 // Remove unread private members
        private readonly Dictionary<string, int> _exact = null;
#pragma warning restore IDE0052 // Remove unread private members

        #region appgen: predefined constructor
        public RoleManagementDetailFilterSpecification() { }

		public RoleManagementDetailFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public RoleManagementDetailFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.RoleManagement);
				Query.Include(e => e.FunctionInfo);

				#endregion
			}
		}

		public RoleManagementDetailFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.RoleManagement);
				Query.Include(e => e.FunctionInfo);

				#endregion
			}
		}

		private readonly int? _skip = null;
		private readonly int? _take = null;
		public RoleManagementDetailFilterSpecification(int skip, int take, Dictionary<string, int> exact)
		{
			_skip = skip;
			_take = take;
			_exact = exact;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<int> RoleManagementIds { get; set; } = new List<int>();
		public List<string> FunctionInfoIds { get; set; } = new List<string>();
		public List<bool> AllowCreates { get; set; } = new List<bool>();
		public List<bool> AllowReads { get; set; } = new List<bool>();
		public List<bool> AllowUpdates { get; set; } = new List<bool>();
		public List<bool> AllowDeletes { get; set; } = new List<bool>();
		public List<bool> ShowInMenus { get; set; } = new List<bool>();
		public List<bool> AllowDownloads { get; set; } = new List<bool>();
		public List<bool> AllowPrints { get; set; } = new List<bool>();
		public List<bool> AllowUploads { get; set; } = new List<bool>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.All;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public RoleManagementDetailFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<RoleManagementDetail>> orderby = null)
		{
			Query.Where(e => (!Id.HasValue || e.Id == Id.Value));
			Query.Where(e => (!MainRecordId.HasValue || e.MainRecordId == MainRecordId));
			if(MainRecordIdIsNull)
				Query.Where(e => e.MainRecordId == null);
			Query.Where(e => (string.IsNullOrEmpty(RecordEditedBy) || e.RecordEditedBy == RecordEditedBy));
			Query.Where(e => (!DraftFromUpload.HasValue || e.DraftFromUpload == DraftFromUpload.Value));

			#region appgen: generated query
			if(Ids?.Count > 0)
				Query.Where(e => Ids.Contains(e.Id));

			if(RoleManagementIds?.Count > 0)
					Query.Where(e => RoleManagementIds.Contains(e.RoleManagementId.Value));

			if(FunctionInfoIds?.Count > 0)
				foreach (var item in FunctionInfoIds)
					Query.Where(e => FunctionInfoIds.Contains(e.FunctionInfoId));

			if(AllowCreates?.Count > 0)
				Query.Where(e => AllowCreates.Contains(e.AllowCreate.Value));

			if(AllowReads?.Count > 0)
				Query.Where(e => AllowReads.Contains(e.AllowRead.Value));

			if(AllowUpdates?.Count > 0)
				Query.Where(e => AllowUpdates.Contains(e.AllowUpdate.Value));

			if(AllowDeletes?.Count > 0)
				Query.Where(e => AllowDeletes.Contains(e.AllowDelete.Value));

			if(ShowInMenus?.Count > 0)
				Query.Where(e => ShowInMenus.Contains(e.ShowInMenu.Value));

			if(AllowDownloads?.Count > 0)
				Query.Where(e => AllowDownloads.Contains(e.AllowDownload.Value));

			if(AllowPrints?.Count > 0)
				Query.Where(e => AllowPrints.Contains(e.AllowPrint.Value));

			if(AllowUploads?.Count > 0)
				Query.Where(e => AllowUploads.Contains(e.AllowUpload.Value));


			#endregion

			if(ShowDraftList > BaseEntity.DraftStatus.All)
				Query.Where(e => e.IsDraftRecord == (int)ShowDraftList);

			if(_skip.HasValue && _take.HasValue)
				Query
					.Skip(_skip.Value)
					.Take(_take.Value);

			if (orderby?.Count > 0)
			{
				foreach(var item in orderby)
				{
					if (item.SortType == SortingType.Ascending)
						Query.OrderBy(item.Predicate);
					else
						Query.OrderByDescending(item.Predicate);
				}
			}

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.RoleManagement);
				Query.Include(e => e.FunctionInfo);

				#endregion
			}

			return this;
		}
		#endregion
	}
}
