using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IAttachmentService : IAsyncBaseService<Attachment>
	{
        Task<Attachment> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Attachment>> ListAllAsync(List<SortingInformation<Attachment>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Attachment>> ListAsync(ISpecification<Attachment> spec, List<SortingInformation<Attachment>> sorting, CancellationToken cancellationToken = default);
        Task<Attachment> AddAsync(Attachment entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Attachment entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Attachment entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Attachment> spec, CancellationToken cancellationToken = default);
        Task<Attachment> FirstAsync(ISpecification<Attachment> spec, CancellationToken cancellationToken = default);
        Task<Attachment> FirstOrDefaultAsync(ISpecification<Attachment> spec, CancellationToken cancellationToken = default);
    }
}
