using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCoreApi.ApplicationCore.Specifications
{
    public class DownloadProcessFilterSpecification : Specification<DownloadProcess>
	{
		private readonly Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public DownloadProcessFilterSpecification() { }

		public DownloadProcessFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public DownloadProcessFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public DownloadProcessFilterSpecification(List<int> ids, bool withBelongsTo = true)
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
		public DownloadProcessFilterSpecification(int skip, int take, Dictionary<string, int> exact)
		{
			_skip = skip;
			_take = take;
			_exact = exact;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<string> JobIds { get; set; } = new List<string>();
		public List<string> FunctionIds { get; set; } = new List<string>();
		public List<string> FileNames { get; set; } = new List<string>();
		public DateTime? StartTimeFrom { get; set; } = null;
		public DateTime? StartTimeTo { get; set; } = null;
		public DateTime? EndTimeFrom { get; set; } = null;
		public DateTime? EndTimeTo { get; set; } = null;
		public List<string> Statuss { get; set; } = new List<string>();
		public List<string> ErrorMessages { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public DownloadProcessFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<DownloadProcess>> orderby = null)
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

			if(JobIds?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("jobid") && _exact["jobid"] == 1)
				{
					Query.Where(e => JobIds.Contains(e.JobId));
				}
				else
				{
					var predicate = PredicateBuilder.False<DownloadProcess>();
					foreach (var item in JobIds)
						predicate = predicate.Or(p => p.JobId.Contains(item));

					Query.Where(predicate);
				}
			}

			if(FunctionIds?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("functionid") && _exact["functionid"] == 1)
				{
					Query.Where(e => FunctionIds.Contains(e.FunctionId));
				}
				else
				{
					var predicate = PredicateBuilder.False<DownloadProcess>();
					foreach (var item in FunctionIds)
						predicate = predicate.Or(p => p.FunctionId.Contains(item));

					Query.Where(predicate);
				}
			}

			if(FileNames?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("filename") && _exact["filename"] == 1)
				{
					Query.Where(e => FileNames.Contains(e.FileName));
				}
				else
				{
					var predicate = PredicateBuilder.False<DownloadProcess>();
					foreach (var item in FileNames)
						predicate = predicate.Or(p => p.FileName.Contains(item));

					Query.Where(predicate);
				}
			}

			if (StartTimeFrom.HasValue || StartTimeTo.HasValue)
				if (StartTimeFrom.HasValue && StartTimeTo.HasValue)
					Query.Where(e => e.StartTime >= StartTimeFrom.Value && e.StartTime <= StartTimeTo.Value);
				else if (StartTimeFrom.HasValue)
					Query.Where(e => e.StartTime >= StartTimeFrom.Value);
				else if (StartTimeTo.HasValue)
					Query.Where(e => e.StartTime <= StartTimeTo.Value);

			if (EndTimeFrom.HasValue || EndTimeTo.HasValue)
				if (EndTimeFrom.HasValue && EndTimeTo.HasValue)
					Query.Where(e => e.EndTime >= EndTimeFrom.Value && e.EndTime <= EndTimeTo.Value);
				else if (EndTimeFrom.HasValue)
					Query.Where(e => e.EndTime >= EndTimeFrom.Value);
				else if (EndTimeTo.HasValue)
					Query.Where(e => e.EndTime <= EndTimeTo.Value);

			if(Statuss?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("status") && _exact["status"] == 1)
				{
					Query.Where(e => Statuss.Contains(e.Status));
				}
				else
				{
					var predicate = PredicateBuilder.False<DownloadProcess>();
					foreach (var item in Statuss)
						predicate = predicate.Or(p => p.Status.Contains(item));

					Query.Where(predicate);
				}
			}

			if(ErrorMessages?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("errormessage") && _exact["errormessage"] == 1)
				{
					Query.Where(e => ErrorMessages.Contains(e.ErrorMessage));
				}
				else
				{
					var predicate = PredicateBuilder.False<DownloadProcess>();
					foreach (var item in ErrorMessages)
						predicate = predicate.Or(p => p.ErrorMessage.Contains(item));

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

		public DownloadProcessFilterSpecification(int id, bool withFunctionInfo = false)
		{
			InitializeSpecification(id: id, withFunctionInfo: withFunctionInfo);
		}

		public DownloadProcessFilterSpecification(int? skip, int? take)
		{
			InitializeSpecification(skip, take);
		}

		public DownloadProcessFilterSpecification(string jobId, string functionId, string status)
		{
			InitializeSpecification(jobId: jobId, functionId: functionId, status: status);
		}

		public DownloadProcessFilterSpecification(int? skip, int? take, string jobId, string functionId, string status)
		{
			InitializeSpecification(skip, take, jobId, functionId, status);
		}

		private void InitializeSpecification(int? skip = null, int? take = null, string jobId = "", string functionId = "", string status = "", int? id = null, bool withFunctionInfo = false)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(jobId) || e.JobId == jobId) &&
					(string.IsNullOrEmpty(functionId) || e.FunctionId == functionId) &&
					(string.IsNullOrEmpty(status) || e.Status == status) &&
					(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withFunctionInfo)
				Query
					.Include(p => p.FunctionInfo);
		}
	}
}
