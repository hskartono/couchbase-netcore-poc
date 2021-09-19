using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCoreApi.ApplicationCore.Specifications.Filters;
using System;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IAsyncRepository<T> where T : CoreEntity
	{
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        [Obsolete("Please use ListAllAsync(CancellationToken token = default)")]
        Task<IReadOnlyList<T>> ListAllAsync(List<SortingInformation<T>> sorting, CancellationToken cancellationToken = default);
        [Obsolete("Please use ListAsync(BaseFilter<T> filter, CancellationToken token = default)")]
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, List<SortingInformation<T>> sorting, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task ReplaceAsync(T entity, int id, CancellationToken cancellationToken = default);
        Task ReplaceAsync(T entity, string id, CancellationToken cancellationToken = default);
        void DeleteAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        void DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

        // filter
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token = default);
        Task<IReadOnlyList<T>> ListAsync(BaseFilter<T> filter, CancellationToken token = default);
        Task DeleteAsync(BaseFilter<T> filter, CancellationToken token = default);
        Task<int> CountAsync(BaseFilter<T> filter, CancellationToken token = default);
        Task<T> FirstAsync(BaseFilter<T> filter, CancellationToken token = default);
        Task<T> FirstOrDefaultAsync(BaseFilter<T> filter, CancellationToken token = default);
    }
}
