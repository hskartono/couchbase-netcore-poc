using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IRoleService : IAsyncBaseService<Role>
	{
        Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Role>> ListAllAsync(List<SortingInformation<Role>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Role>> ListAsync(ISpecification<Role> spec, List<SortingInformation<Role>> sorting, CancellationToken cancellationToken = default);
        Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Role entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Role entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
        Task<Role> FirstAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
        Task<Role> FirstOrDefaultAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
        Task<Role> GetUserRole(string userName, CancellationToken cancellationToken = default);
        Task<Dictionary<string, Role>> GetUserRoles(string userName, CancellationToken cancellationToken = default);
        Task<Role> GetUserGroup(string userName, CancellationToken cancellationToken = default);
    }
}
