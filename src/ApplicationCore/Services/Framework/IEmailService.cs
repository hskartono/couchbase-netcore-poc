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
	public interface IEmailService : IAsyncBaseService<Email>
	{
        Task<Email> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Email>> ListAllAsync(List<SortingInformation<Email>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Email>> ListAsync(ISpecification<Email> spec, List<SortingInformation<Email>> sorting, CancellationToken cancellationToken = default);
        Task<Email> AddAsync(Email entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Email entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Email entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Email> spec, CancellationToken cancellationToken = default);
        Task<Email> FirstAsync(ISpecification<Email> spec, CancellationToken cancellationToken = default);
        Task<Email> FirstOrDefaultAsync(ISpecification<Email> spec, CancellationToken cancellationToken = default);

        Task<Email> SendEmail(
            List<String> receiver,
            string subject,
            string templateFileName,
            Dictionary<String, String> contentMap,
            List<Attachment> attachments, CancellationToken cancellationToken = default);
        Task<Email> SendEmail(
            List<String> receiver,
            List<String> carbonCopy,
            string subject,
            string templateFileName,
            Dictionary<String, String> contentMap,
            List<Attachment> attachments, CancellationToken cancellationToken = default);

        Task ProcessEmailBatch();
    }
}
