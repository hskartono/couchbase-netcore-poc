using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IRoleRepository : IAsyncRepository<Role>
	{
		Task<Role> GetUserGroup(string userName, CancellationToken cancellationToken = default);
	}
}
