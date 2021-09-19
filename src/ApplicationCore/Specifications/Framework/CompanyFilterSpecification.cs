using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class CompanyFilterSpecification : Specification<Company>
	{
		public CompanyFilterSpecification(int id)
		{
			InitializeSpecification(id: id);
		}

		public CompanyFilterSpecification(int skip, int take)
		{
			InitializeSpecification(skip, take);
		}

		public CompanyFilterSpecification(int skip, int take, string name)
		{
			InitializeSpecification(skip, take, name);
		}

		public CompanyFilterSpecification(string name)
		{
			InitializeSpecification(name: name);
		}

		private void InitializeSpecification(int? skip = null, int? take = null, string name = "", int? id = null)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
					(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
