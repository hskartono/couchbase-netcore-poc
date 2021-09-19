using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class LookupDetailFilterSpecification : Specification<LookupDetail>
	{
		public LookupDetailFilterSpecification(int id)
		{
			Query.Where(e => e.Id == id);
		}

		public LookupDetailFilterSpecification(string name)
		{
			Query.Where(e => e.Name == name);
		}

		public LookupDetailFilterSpecification(string name, int lookupId)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(name) || e.Name == name) &&
					(e.LookupId == lookupId)
				);
		}

		private void InitializeSpecification(int? skip=null, int? take=null, string name="", string value="", int? lookupId = null)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
					(string.IsNullOrEmpty(value) || e.Value.Contains(value)) &&
					(!lookupId.HasValue || e.LookupId == lookupId)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
