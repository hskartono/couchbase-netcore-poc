using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IFunctionInfoService : IAsyncBaseService<FunctionInfo>
	{
        Task<FunctionInfo> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FunctionInfo>> ListAllAsync(List<SortingInformation<FunctionInfo>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FunctionInfo>> ListAsync(ISpecification<FunctionInfo> spec, List<SortingInformation<FunctionInfo>> sorting, CancellationToken cancellationToken = default);
        Task<FunctionInfo> AddAsync(FunctionInfo entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(FunctionInfo entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(FunctionInfo entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default);
        Task<FunctionInfo> FirstAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default);
        Task<FunctionInfo> FirstOrDefaultAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default);
    }
}
