using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface ILookupService
	{
        Task<Lookup> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Lookup>> ListAllAsync(List<SortingInformation<Lookup>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Lookup>> ListAsync(ISpecification<Lookup> spec, List<SortingInformation<Lookup>> sorting, CancellationToken cancellationToken = default);
        Task<Lookup> AddAsync(Lookup entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Lookup entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Lookup entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Lookup> spec, CancellationToken cancellationToken = default);
        Task<Lookup> FirstAsync(ISpecification<Lookup> spec, CancellationToken cancellationToken = default);
        Task<Lookup> FirstOrDefaultAsync(ISpecification<Lookup> spec, CancellationToken cancellationToken = default);
    }
}
