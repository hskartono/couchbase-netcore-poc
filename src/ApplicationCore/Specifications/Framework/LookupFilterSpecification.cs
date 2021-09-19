using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class LookupFilterSpecification : Specification<Lookup>
	{
		public LookupFilterSpecification(int id, bool withChild = true)
		{
			InitializeSpecification(id: id, withChild: withChild);
		}

		public LookupFilterSpecification(string name)
		{
			Query.Where(e => e.Name == name);
			Query.Include(e => e.LookupDetails);
		}

		public LookupFilterSpecification(int skip, int take)
		{
			InitializeSpecification(skip, take);
		}

		public LookupFilterSpecification(int skip, int take, string name = "", bool withChild = false)
		{
			InitializeSpecification(skip, take, name, withChild: withChild);
		}

		private void InitializeSpecification(int? skip=null, int? take=null, string name = "", int? id = null, bool withChild = false)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
					(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withChild)
				Query
					.Include(e => e.LookupDetails);
		}
	}
}
