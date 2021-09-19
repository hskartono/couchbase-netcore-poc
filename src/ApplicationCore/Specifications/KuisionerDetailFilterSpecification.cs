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
	public class KuisionerDetailFilterSpecification : Specification<KuisionerDetail>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public KuisionerDetailFilterSpecification() { }

		public KuisionerDetailFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public KuisionerDetailFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.Kuisioner);
				Query.Include(e => e.Soal);

				#endregion
			}
		}

		public KuisionerDetailFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.Kuisioner);
				Query.Include(e => e.Soal);

				#endregion
			}
		}

		private int? _skip = null;
		private int? _take = null;
		public KuisionerDetailFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<int> KuisionerIds { get; set; } = new List<int>();
		public List<string> SoalIds { get; set; } = new List<string>();
		public List<string> KontenSoals { get; set; } = new List<string>();
		public List<string> Pilihan1s { get; set; } = new List<string>();
		public List<string> PIlihan2s { get; set; } = new List<string>();
		public List<string> Pilihan3s { get; set; } = new List<string>();
		public List<int> KunciJawabans { get; set; } = new List<int>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.All;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public KuisionerDetailFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<KuisionerDetail>> orderby = null)
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

			if(KuisionerIds?.Count > 0)
					Query.Where(e => KuisionerIds.Contains(e.KuisionerId.Value));

			if(SoalIds?.Count > 0)
				foreach (var item in SoalIds)
					Query.Where(e => SoalIds.Contains(e.SoalId));

			if(KontenSoals?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("kontensoal") && _exact["kontensoal"] == 1)
				{
					Query.Where(e => KontenSoals.Contains(e.KontenSoal));
				}
				else
				{
					var predicate = PredicateBuilder.False<KuisionerDetail>();
					foreach (var item in KontenSoals)
						predicate = predicate.Or(p => p.KontenSoal.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Pilihan1s?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("pilihan1") && _exact["pilihan1"] == 1)
				{
					Query.Where(e => Pilihan1s.Contains(e.Pilihan1));
				}
				else
				{
					var predicate = PredicateBuilder.False<KuisionerDetail>();
					foreach (var item in Pilihan1s)
						predicate = predicate.Or(p => p.Pilihan1.Contains(item));

					Query.Where(predicate);
				}
			}

			if(PIlihan2s?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("pilihan2") && _exact["pilihan2"] == 1)
				{
					Query.Where(e => PIlihan2s.Contains(e.PIlihan2));
				}
				else
				{
					var predicate = PredicateBuilder.False<KuisionerDetail>();
					foreach (var item in PIlihan2s)
						predicate = predicate.Or(p => p.PIlihan2.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Pilihan3s?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("pilihan3") && _exact["pilihan3"] == 1)
				{
					Query.Where(e => Pilihan3s.Contains(e.Pilihan3));
				}
				else
				{
					var predicate = PredicateBuilder.False<KuisionerDetail>();
					foreach (var item in Pilihan3s)
						predicate = predicate.Or(p => p.Pilihan3.Contains(item));

					Query.Where(predicate);
				}
			}

			if(KunciJawabans?.Count > 0)
				Query.Where(e => KunciJawabans.Contains(e.KunciJawaban.Value));


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
				Query.Include(e => e.Kuisioner);
				Query.Include(e => e.Soal);

				#endregion
			}

			return this;
		}
		#endregion
	}
}
