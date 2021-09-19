using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface ICompanyService : IAsyncBaseService<Company>
	{
        Task<Company> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Company>> ListAllAsync(List<SortingInformation<Company>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Company>> ListAsync(ISpecification<Company> spec, List<SortingInformation<Company>> sorting, CancellationToken cancellationToken = default);
        Task<Company> AddAsync(Company entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Company entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Company entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Company> spec, CancellationToken cancellationToken = default);
        Task<Company> FirstAsync(ISpecification<Company> spec, CancellationToken cancellationToken = default);
        Task<Company> FirstOrDefaultAsync(ISpecification<Company> spec, CancellationToken cancellationToken = default);
    }
}
