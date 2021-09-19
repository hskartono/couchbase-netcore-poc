using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IUserRoleService : IAsyncBaseService<UserRole>
	{
        Task<UserRole> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserRole>> ListAllAsync(List<SortingInformation<UserRole>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserRole>> ListAsync(ISpecification<UserRole> spec, List<SortingInformation<UserRole>> sorting, CancellationToken cancellationToken = default);
        Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(UserRole entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(UserRole entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<UserRole> spec, CancellationToken cancellationToken = default);
        Task<UserRole> FirstAsync(ISpecification<UserRole> spec, CancellationToken cancellationToken = default);
        Task<UserRole> FirstOrDefaultAsync(ISpecification<UserRole> spec, CancellationToken cancellationToken = default);
    }
}
