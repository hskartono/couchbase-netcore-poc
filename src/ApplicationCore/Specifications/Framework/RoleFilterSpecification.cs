using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using System.Collections.Generic;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class RoleFilterSpecification : Specification<Role>
	{
		public RoleFilterSpecification(int id, bool withChild = true)
		{
			InitializeFilterData(id: id, withChild: withChild);
		}

		public RoleFilterSpecification(List<int> ids, bool withChild = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withChild)
				Query.Include(e => e.RoleDetails)
					.ThenInclude(d=>d.FunctionInfo)
					.ThenInclude(d=>d.ModuleInfo);
		}

		public RoleFilterSpecification(int skip, int take, bool withChild = false)
		{
			InitializeFilterData(skip, take, withChild: withChild);
		}

		public RoleFilterSpecification(int skip, int take, string name, string description, bool withChild = false)
		{
			InitializeFilterData(skip, take, name, description, withChild: withChild);
		}

		public RoleFilterSpecification(string name, string description, bool withChild = false)
		{
			InitializeFilterData(name: name, description: description, withChild: withChild);
		}

		protected void InitializeFilterData(int? skip = null, int? take = null, 
			string name = "", string description = "",
			int? id = null, 
			bool withChild = false)
		{
			Query
				.Where(
					e => (string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
					(string.IsNullOrEmpty(description) || e.Description.Contains(description)) &&
					(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			// include detail-nya, dan object detail lainnya selain parent
			if(withChild)
				Query
					.Include(e => e.RoleDetails)
					.ThenInclude(d => d.FunctionInfo);
		}
	}
}
