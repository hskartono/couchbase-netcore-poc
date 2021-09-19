using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Specifications
{
    public class SchedulerConfigurationFilterSpecification : Specification<SchedulerConfiguration>
	{
		private readonly Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public SchedulerConfigurationFilterSpecification() { }

		public SchedulerConfigurationFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public SchedulerConfigurationFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.IntervalType);
				Query.Include(e => e.JobType);

				#endregion
			}
		}

		public SchedulerConfigurationFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.IntervalType);
				Query.Include(e => e.JobType);

				#endregion
			}
		}

		private readonly int? _skip = null;
		private readonly int? _take = null;
		public SchedulerConfigurationFilterSpecification(int skip, int take, Dictionary<string, int> exact)
		{
			_skip = skip;
			_take = take;
			_exact = exact;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<string> IntervalTypeIds { get; set; } = new List<string>();
		public List<int> IntervalValues { get; set; } = new List<int>();
		public List<int> IntervalValue2s { get; set; } = new List<int>();
		public List<int> IntervalValue3s { get; set; } = new List<int>();
		public List<string> CronExpressions { get; set; } = new List<string>();
		public List<int> JobTypeIds { get; set; } = new List<int>();
		public List<string> RecurringJobIds { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public SchedulerConfigurationFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<SchedulerConfiguration>> orderby = null)
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

			if(IntervalTypeIds?.Count > 0)
				foreach (var item in IntervalTypeIds)
					Query.Where(e => IntervalTypeIds.Contains(e.IntervalTypeId));

			if(IntervalValues?.Count > 0)
				Query.Where(e => IntervalValues.Contains(e.IntervalValue.Value));

			if(IntervalValue2s?.Count > 0)
				Query.Where(e => IntervalValue2s.Contains(e.IntervalValue2.Value));

			if(IntervalValue3s?.Count > 0)
				Query.Where(e => IntervalValue3s.Contains(e.IntervalValue3.Value));

			if(CronExpressions?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("cronexpression") && _exact["cronexpression"] == 1)
				{
					Query.Where(e => CronExpressions.Contains(e.CronExpression));
				}
				else
				{
					var predicate = PredicateBuilder.False<SchedulerConfiguration>();
					foreach (var item in CronExpressions)
						predicate = predicate.Or(p => p.CronExpression.Contains(item));

					Query.Where(predicate);
				}
			}

			if(JobTypeIds?.Count > 0)
					Query.Where(e => JobTypeIds.Contains(e.JobTypeId.Value));

			if(RecurringJobIds?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("recurringjobid") && _exact["recurringjobid"] == 1)
				{
					Query.Where(e => RecurringJobIds.Contains(e.RecurringJobId));
				}
				else
				{
					var predicate = PredicateBuilder.False<SchedulerConfiguration>();
					foreach (var item in RecurringJobIds)
						predicate = predicate.Or(p => p.RecurringJobId.Contains(item));

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
				Query.Include(e => e.IntervalType);
				Query.Include(e => e.JobType);

				#endregion
			}

			return this;
		}
		#endregion
	}
}
