using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Specifications.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class KuisionerFilterSpecification : Specification<Kuisioner>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public KuisionerFilterSpecification() { }

		public KuisionerFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public KuisionerFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public KuisionerFilterSpecification(List<int> ids, bool withBelongsTo = true)
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
		public KuisionerFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<string> Juduls { get; set; } = new List<string>();
		public DateTime? AktifDariFrom { get; set; } = null;
		public DateTime? AktifDariTo { get; set; } = null;
		public DateTime? AktifSampaiFrom { get; set; } = null;
		public DateTime? AktifSampaiTo { get; set; } = null;

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public KuisionerFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<Kuisioner>> orderby = null)
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

			if(Juduls?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("judul") && _exact["judul"] == 1)
				{
					Query.Where(e => Juduls.Contains(e.Judul));
				}
				else
				{
					var predicate = PredicateBuilder.False<Kuisioner>();
					foreach (var item in Juduls)
						predicate = predicate.Or(p => p.Judul.Contains(item));

					Query.Where(predicate);
				}
			}

			if (AktifDariFrom.HasValue || AktifDariTo.HasValue)
				if (AktifDariFrom.HasValue && AktifDariTo.HasValue)
					Query.Where(e => e.AktifDari >= AktifDariFrom.Value && e.AktifDari <= AktifDariTo.Value);
				else if (AktifDariFrom.HasValue)
					Query.Where(e => e.AktifDari >= AktifDariFrom.Value);
				else if (AktifDariTo.HasValue)
					Query.Where(e => e.AktifDari <= AktifDariTo.Value);

			if (AktifSampaiFrom.HasValue || AktifSampaiTo.HasValue)
				if (AktifSampaiFrom.HasValue && AktifSampaiTo.HasValue)
					Query.Where(e => e.AktifSampai >= AktifSampaiFrom.Value && e.AktifSampai <= AktifSampaiTo.Value);
				else if (AktifSampaiFrom.HasValue)
					Query.Where(e => e.AktifSampai >= AktifSampaiFrom.Value);
				else if (AktifSampaiTo.HasValue)
					Query.Where(e => e.AktifSampai <= AktifSampaiTo.Value);


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
