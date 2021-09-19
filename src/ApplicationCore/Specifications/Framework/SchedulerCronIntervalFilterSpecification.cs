using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Specifications
{
    public class SchedulerCronIntervalFilterSpecification : Specification<SchedulerCronInterval>
	{
		private readonly Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public SchedulerCronIntervalFilterSpecification() { }

		public SchedulerCronIntervalFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public SchedulerCronIntervalFilterSpecification(string id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public SchedulerCronIntervalFilterSpecification(List<string> ids, bool withBelongsTo = true)
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
		public SchedulerCronIntervalFilterSpecification(int skip, int take, Dictionary<string, int> exact)
		{
			_skip = skip;
			_take = take;
			_exact = exact;
		}
		#endregion

		#region appgen: generated property list
		public string Id { get; set; }
		public List<string> Ids { get; set; } = new List<string>();
		public List<string> Names { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public string MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public SchedulerCronIntervalFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<SchedulerCronInterval>> orderby = null)
		{
			Query.Where(e => (string.IsNullOrEmpty(Id) || e.Id == Id));
			Query.Where(e => (string.IsNullOrEmpty(MainRecordId) || e.MainRecordId == MainRecordId));
			if(MainRecordIdIsNull)
				Query.Where(e => e.MainRecordId == null);
			Query.Where(e => (string.IsNullOrEmpty(RecordEditedBy) || e.RecordEditedBy == RecordEditedBy));
			Query.Where(e => (!DraftFromUpload.HasValue || e.DraftFromUpload == DraftFromUpload.Value));

			#region appgen: generated query
			if(Ids?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("id") && _exact["id"] == 1)
				{
					Query.Where(e => Ids.Contains(e.Id));
				}
				else
				{
					var predicate = PredicateBuilder.False<SchedulerCronInterval>();
					foreach (var item in Ids)
						predicate = predicate.Or(p => p.Id.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Names?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("name") && _exact["name"] == 1)
				{
					Query.Where(e => Names.Contains(e.Name));
				}
				else
				{
					var predicate = PredicateBuilder.False<SchedulerCronInterval>();
					foreach (var item in Names)
						predicate = predicate.Or(p => p.Name.Contains(item));

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
