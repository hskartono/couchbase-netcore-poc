using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Specifications
{
    public class JobConfigurationFilterSpecification : Specification<JobConfiguration>
	{
		private readonly Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public JobConfigurationFilterSpecification() { }

		public JobConfigurationFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public JobConfigurationFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public JobConfigurationFilterSpecification(List<int> ids, bool withBelongsTo = true)
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
		public JobConfigurationFilterSpecification(int skip, int take, Dictionary<string, int> exact)
		{
			_skip = skip;
			_take = take;
			_exact = exact;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<string> InterfaceNames { get; set; } = new List<string>();
		public List<string> JobNames { get; set; } = new List<string>();
		public List<string> JobDescriptions { get; set; } = new List<string>();
		public List<bool> IsStoredProcedures { get; set; } = new List<bool>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public JobConfigurationFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<JobConfiguration>> orderby = null)
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

			if(InterfaceNames?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("interfacename") && _exact["interfacename"] == 1)
				{
					Query.Where(e => InterfaceNames.Contains(e.InterfaceName));
				}
				else
				{
					var predicate = PredicateBuilder.False<JobConfiguration>();
					foreach (var item in InterfaceNames)
						predicate = predicate.Or(p => p.InterfaceName.Contains(item));

					Query.Where(predicate);
				}
			}

			if(JobNames?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("jobname") && _exact["jobname"] == 1)
				{
					Query.Where(e => JobNames.Contains(e.JobName));
				}
				else
				{
					var predicate = PredicateBuilder.False<JobConfiguration>();
					foreach (var item in JobNames)
						predicate = predicate.Or(p => p.JobName.Contains(item));

					Query.Where(predicate);
				}
			}

			if(JobDescriptions?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("jobdescription") && _exact["jobdescription"] == 1)
				{
					Query.Where(e => JobDescriptions.Contains(e.JobDescription));
				}
				else
				{
					var predicate = PredicateBuilder.False<JobConfiguration>();
					foreach (var item in JobDescriptions)
						predicate = predicate.Or(p => p.JobDescription.Contains(item));

					Query.Where(predicate);
				}
			}

			if(IsStoredProcedures?.Count > 0)
				Query.Where(e => IsStoredProcedures.Contains(e.IsStoredProcedure.Value));


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
