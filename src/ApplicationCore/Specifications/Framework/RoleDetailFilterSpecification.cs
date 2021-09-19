using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Specifications
{
	public class RoleDetailFilterSpecification : Specification<RoleDetail>
	{
		public RoleDetailFilterSpecification(int roleId, bool? withParent = false)
		{
			InitializeFilterData(roleId: roleId, withParent: (withParent.HasValue && withParent.Value));
		}

		public RoleDetailFilterSpecification(int skip, int size, int roleId, bool? withParent = false)
		{
			InitializeFilterData(skip, Take, roleId, withParent: (withParent.HasValue && withParent.Value));
		}

		private void InitializeFilterData(int? skip = null, int? take = null, int? roleId = null, bool withParent = false)
		{
			Query
				.Where(p => p.RoleId == roleId);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withParent)
				Query.Include(p => p.Role);
		}
	}
}
