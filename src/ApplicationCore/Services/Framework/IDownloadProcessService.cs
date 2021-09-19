using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IDownloadProcessService : IAsyncBaseService<DownloadProcess>
	{
        Task<DownloadProcess> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DownloadProcess>> ListAllAsync(List<SortingInformation<DownloadProcess>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DownloadProcess>> ListAsync(ISpecification<DownloadProcess> spec, List<SortingInformation<DownloadProcess>> sorting, CancellationToken cancellationToken = default);
        Task<DownloadProcess> AddAsync(DownloadProcess entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(DownloadProcess entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(DownloadProcess entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<DownloadProcess> spec, CancellationToken cancellationToken = default);
        Task<DownloadProcess> FirstAsync(ISpecification<DownloadProcess> spec, CancellationToken cancellationToken = default);
        Task<DownloadProcess> FirstOrDefaultAsync(ISpecification<DownloadProcess> spec, CancellationToken cancellationToken = default);
        Task SuccessfullyGenerated(int Id, string fileName, CancellationToken cancellationToken = default);
        Task FailedToGenerate(int Id, string description, CancellationToken cancellationToken = default);
        Task<string> RegisterDownloadProcess(string functionId, string jobId, CancellationToken cancellationToken = default);
        Task<DownloadProcess> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default);
    }
}
