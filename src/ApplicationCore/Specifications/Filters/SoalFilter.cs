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
    public class SoalFilter : BaseFilter<Soal>
    {
        public SoalFilter()
        {
            PageSize = 0;
            PageIndex = 0;
        }

        public SoalFilter(int pageSize, int pageIndex)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public SoalFilter(Dictionary<string, List<string>> filters, int pageSize, int pageIndex) : base(filters)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }

        public override ISpecification<Soal> ToSpecification(bool withBelongsTo = true)
        {
            var specification = new SoalFilterSpecification(PageIndex, PageSize);

            return specification.BuildSpecification(withBelongsTo, SortingSpec);
        }

        public string IdEqual { get; set; }
        public string IdContain { get; set; }
        public string KontenEqual { get; set; }
        public string KontenContain { get; set; }
        public List<string> KontenEquals { get; set; } = new List<string>();
        public List<string> KontenContains { get; set; } = new List<string>();
        public string MainRecordId { get; set; }

    }
}
