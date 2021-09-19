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
	public class SoalFilterSpecification : Specification<Soal>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public SoalFilterSpecification() { }

		public SoalFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public SoalFilterSpecification(string id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public SoalFilterSpecification(List<string> ids, bool withBelongsTo = true)
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
		public SoalFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public string Id { get; set; }
		public List<string> Ids { get; set; } = new List<string>();
		public List<string> Kontens { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public string MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public SoalFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<Soal>> orderby = null)
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
					var predicate = PredicateBuilder.False<Soal>();
					foreach (var item in Ids)
						predicate = predicate.Or(p => p.Id.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Kontens?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("konten") && _exact["konten"] == 1)
				{
					Query.Where(e => Kontens.Contains(e.Konten));
				}
				else
				{
					var predicate = PredicateBuilder.False<Soal>();
					foreach (var item in Kontens)
						predicate = predicate.Or(p => p.Konten.Contains(item));

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
