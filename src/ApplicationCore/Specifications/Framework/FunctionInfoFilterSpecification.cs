using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class FunctionInfoFilterSpecification : Specification<FunctionInfo>
	{
		private Dictionary<string, int> _exact = null;

        public FunctionInfoFilterSpecification()
        {

        }

		public FunctionInfoFilterSpecification(int skip, int take)
		{
			_take = take;
			_skip = skip;
			Query
				.Skip(skip)
				.Take(take);
		}

		public FunctionInfoFilterSpecification(int skip, int take, string name, bool? isEnabled)
		{
			InitializeSpecification(skip, take, name, isEnabled);
		}

		public FunctionInfoFilterSpecification(string name, bool? isEnabled)
		{
			InitializeSpecification(name: name, isEnabled: isEnabled);
		}

		public FunctionInfoFilterSpecification(string id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.ModuleInfo);

				#endregion
			}
		}

		public FunctionInfoFilterSpecification(List<string> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.ModuleInfo);
				#endregion
			}
		}

		private int? _skip = null;
		private int? _take = null;
		private void InitializeSpecification(int? skip = null, int? take = null, string name = "", bool? isEnabled = false)
		{
			Query
				.Where(
					e => (string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
					(!isEnabled.HasValue || e.IsEnabled == isEnabled)
				);

			if(skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}

		#region appgen: generated property list
		public int Id { get; set; }
		public List<int> Ids { get; set; } = null;
		public List<string> Names { get; set; } = null;
		public List<string> Uris { get; set; } = null;
		public List<string> IconNames { get; set; } = null;
		public List<bool> IsEnableds { get; set; } = new List<bool>();
		public List<int> ModuleInfoIds { get; set; } = new List<int>();

        public string NameContains { get; set; }
        public string NameEqual { get; set; }
        public string uriContains { get; set; }
        public string iconNameContains { get; set; }
        public bool? IsEnabled { get; set; }
        public int? ModuleInfoId { get; set; }

        #endregion

        #region appgen: recovery property list
        public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public FunctionInfoFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<FunctionInfo>> orderby = null)
		{
			if (ModuleInfoIds?.Count > 0)
				Query.Where(e => ModuleInfoIds.Contains(e.ModuleInfoId.Value));


			if (Names?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("name") && _exact["name"] == 1)
				{
					Query.Where(e => Names.Contains(e.Name));
				}
				else
				{
					var predicate = PredicateBuilder.False<FunctionInfo>();
					foreach (var item in Names)
						predicate = predicate.Or(e => EF.Functions.Like(e.Name, $"%{item}%"));

					Query.Where(predicate);
				}
			}

			if (Uris?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("uri") && _exact["uri"] == 1)
				{
					Query.Where(e => Uris.Contains(e.Name));
				}
				else
				{
					var predicate = PredicateBuilder.False<FunctionInfo>();
					foreach (var item in Uris)
						predicate = predicate.Or(e => EF.Functions.Like(e.Uri, $"%{item}%"));

					Query.Where(predicate);
				}
			}

			if (IsEnableds?.Count > 0)
				Query.Where(e => IsEnableds.Contains(e.IsEnabled.Value));

            if (!string.IsNullOrEmpty(NameEqual))
            {
				Query.Where(e => e.Name == NameEqual);
            }

            if (!string.IsNullOrEmpty(NameContains))
            {
				Query.Where(e => e.Name.Contains(NameContains));
            }

            if (IsEnabled.HasValue)
            {
				Query.Where(e => e.IsEnabled.HasValue && e.IsEnabled.Value == IsEnabled.Value);
            }

            if (ModuleInfoId.HasValue)
            {
				Query.Where(e => e.ModuleInfoId == ModuleInfoId.Value);
            }

			if (!string.IsNullOrEmpty(uriContains))
				Query.Where(e => e.Uri.Contains(uriContains));

			if (!string.IsNullOrEmpty(iconNameContains))
				Query.Where(e => e.IconName.Contains(iconNameContains));

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.ModuleInfo);
				#endregion
			}

			return this;
		}
		#endregion

	}
}
