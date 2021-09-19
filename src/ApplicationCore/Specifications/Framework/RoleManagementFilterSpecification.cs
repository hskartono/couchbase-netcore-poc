using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Specifications
{
    public class RoleManagementFilterSpecification : Specification<RoleManagement>
	{
		private readonly Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public RoleManagementFilterSpecification() { }

		public RoleManagementFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public RoleManagementFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public RoleManagementFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		private readonly int? _skip = null;
		private readonly int? _take = null;
		public RoleManagementFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
			_exact = exact;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<string> Names { get; set; } = new List<string>();
		public List<string> Descriptions { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public RoleManagementFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<RoleManagement>> orderby = null)
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

			if(Names?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("name") && _exact["name"] == 1)
				{
					Query.Where(e => Names.Contains(e.Name));
				}
				else
				{
					var predicate = PredicateBuilder.False<RoleManagement>();
					foreach (var item in Names)
						predicate = predicate.Or(p => p.Name.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Descriptions?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("description") && _exact["description"] == 1)
				{
					Query.Where(e => Descriptions.Contains(e.Description));
				}
				else
				{
					var predicate = PredicateBuilder.False<RoleManagement>();
					foreach (var item in Descriptions)
						predicate = predicate.Or(p => p.Description.Contains(item));

					Query.Where(predicate);
				}
			}


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

				#endregion
			}

			return this;
		}
		#endregion
	}
}
