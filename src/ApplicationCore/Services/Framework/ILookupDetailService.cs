using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface ILookupDetailService
	{
        Task<LookupDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LookupDetail>> ListAllAsync(List<SortingInformation<LookupDetail>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LookupDetail>> ListAsync(ISpecification<LookupDetail> spec, List<SortingInformation<LookupDetail>> sorting, CancellationToken cancellationToken = default);
        Task<LookupDetail> AddAsync(LookupDetail entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(LookupDetail entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(LookupDetail entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<LookupDetail> spec, CancellationToken cancellationToken = default);
        Task<LookupDetail> FirstAsync(ISpecification<LookupDetail> spec, CancellationToken cancellationToken = default);
        Task<LookupDetail> FirstOrDefaultAsync(ISpecification<LookupDetail> spec, CancellationToken cancellationToken = default);
    }
}
