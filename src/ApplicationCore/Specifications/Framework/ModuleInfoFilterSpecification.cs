using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class ModuleInfoFilterSpecification : Specification<ModuleInfo>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public ModuleInfoFilterSpecification() { }

		public ModuleInfoFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public ModuleInfoFilterSpecification(int id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public ModuleInfoFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		private int? _skip = null;
		private int? _take = null;
		public ModuleInfoFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = null;
		public List<string> Names { get; set; } = null;
		public List<int> ParentModuleIds { get; set; } = null;

		//Tambahan REZA
		public List<int> OrderPositions { get; set; } = null;

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public ModuleInfoFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<ModuleInfo>> orderby = null)
		{
			Query.Where(e => (!Id.HasValue || e.Id == Id.Value));
			Query.Where(e => (!MainRecordId.HasValue || e.MainRecordId == MainRecordId.Value));
			if(MainRecordIdIsNull)
				Query.Where(e => e.MainRecordId == null);
			Query.Where(e => (string.IsNullOrEmpty(RecordEditedBy) || e.RecordEditedBy == RecordEditedBy));
			Query.Where(e => (!DraftFromUpload.HasValue || e.DraftFromUpload == DraftFromUpload.Value));

			#region appgen: generated query
			if(Ids?.Count > 0)
				foreach (var item in Ids)
					Query.Where(e => e.Id == item);
			if(Names?.Count > 0)
				foreach (var item in Names)
					if (_exact != null && _exact.ContainsKey("name") && _exact["name"] == 1)
						Query.Where(e => (string.IsNullOrEmpty(item) || e.Name == item));
					else
						Query.Where(e => (string.IsNullOrEmpty(item) || e.Name.Contains(item)));
			if(ParentModuleIds?.Count > 0)
				foreach (var item in ParentModuleIds)
					Query.Where(e => e.ParentModuleId == item);

			//TAMBAHAN REZA
			if (OrderPositions?.Count > 0)
				foreach (var item in OrderPositions)
					Query.Where(e => e.OrderPosition == item);

			#endregion

			if (ShowDraftList > BaseEntity.DraftStatus.All)
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

				#endregion
			}

			return this;
		}
		#endregion
	}
}
