using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Specifications.Filters
{
    public abstract class BaseFilter<T> where T: CoreEntity
    {
        public BaseFilter()
        {

        }

        public BaseFilter(Dictionary<string, List<string>> filters)
        {
            this.MainRecordIsNull = (filters.ContainsKey("mainRecordIsNull") && bool.Parse(filters["mainRecordIsNull"][0]));
            this.RecordEditedBy = (filters.ContainsKey("recordEditedBy") ? filters["recordEditedBy"][0] : null);
            if (filters.ContainsKey("draftFromUpload"))
            {
                if (filters["draftFromUpload"][0] == "1")
                    filters["draftFromUpload"][0] = "true";
                else
                    filters["draftFromUpload"][0] = "false";
            }
            this.IsDraftFromUpload = (filters.ContainsKey("draftFromUpload") ? bool.Parse(filters["draftFromUpload"][0]) : null);

            if (filters.ContainsKey("draftMode"))
            {
                if (filters["draftMode"][0] == "true") filters["draftMode"][0] = "1";
                if (filters["draftMode"][0] == "false") filters["draftMode"][0] = "0";
            }
            this.DraftMode = (filters.ContainsKey("draftMode") ? int.Parse(filters["draftMode"][0]) : 0);
        }
        public bool MainRecordIsNull { get; set; }
        public string RecordEditedBy { get; set; }
        public bool? IsDraftFromUpload { get; set; }
        public int DraftMode { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public List<SortingInformation<T>> SortingSpec { get; set; }

        public abstract ISpecification<T> ToSpecification(bool withBelongsTo = true);
    }
}
