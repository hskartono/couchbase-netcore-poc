using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Specifications.Filters;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
    public class AsyncCouchbaseRepository<T> : IAsyncRepository<T> where T : CoreEntity
    {
        protected readonly ICouchbaseRepository _couchbaseRepository;
        protected const string CB_COLLECTION = "jejak_pendapat";

        public AsyncCouchbaseRepository(ICouchbaseRepository couchbaseRepository)
        {
            _couchbaseRepository = couchbaseRepository;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var propInfo = typeof(T).GetProperty("Id");
            var id = propInfo.GetValue(entity);
            var scope = await _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION);
            var collections = await scope.CollectionAsync(colName);
            if ((string)id == "") id = Guid.NewGuid();
            string key = $"{colName}::{id}";
            var inserted = await collections.InsertAsync<T>(key, entity);
            return entity;
        }

        public Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var propInfo = typeof(T).GetProperty("Id");
            var id = propInfo.GetValue(entity);
            var scope = _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION).Result;
            var collections = scope.CollectionAsync(colName).Result;
            string key = $"{colName}::{id}";
            collections.RemoveAsync(key).ConfigureAwait(false);
        }

        public Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var scope = await _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION);
            var collections = await scope.CollectionAsync(colName);
            string key = $"{colName}::{id}";
            var result = await collections.GetAsync(key);
            return result.ContentAs<T>();
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var scope = await _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION);
            var collections = await scope.CollectionAsync(colName);
            string key = $"{colName}::{id}";
            var result = await collections.GetAsync(key);
            return result.ContentAs<T>();
        }

        public Task<IReadOnlyList<T>> ListAllAsync(List<SortingInformation<T>> sorting, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, List<SortingInformation<T>> sorting, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task ReplaceAsync(T entity, int id, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var scope = await _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION);
            var collections = await scope.CollectionAsync(colName);
            string key = $"{colName}::{id}";
            await collections.ReplaceAsync<T>(key, entity);
        }

        public async Task ReplaceAsync(T entity, string id, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var scope = await _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION);
            var collections = await scope.CollectionAsync(colName);
            string key = $"{colName}::{id}";
            await collections.ReplaceAsync<T>(key, entity);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var colName = typeof(T).Name.ToLower();
            var propInfo = typeof(T).GetProperty("Id");
            var id = propInfo.GetValue(entity);
            var scope = await _couchbaseRepository.DefaultBucket.ScopeAsync(CB_COLLECTION);
            var collections = await scope.CollectionAsync(colName);
            string key = $"{colName}::{id}";
            await collections.ReplaceAsync<T>(key, entity);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token = default)
        {
            // TODO filter belum

            var colName = typeof(T).Name.ToLower();
            var bucketName = _couchbaseRepository.DefaultBucket.Name;
            var tableName = $"`{bucketName}`.{CB_COLLECTION}.{colName}";
            var sql = $"SELECT g.* FROM {tableName} AS g";
            var result = await _couchbaseRepository.Cluster.QueryAsync<T>(sql);
            if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
            {
                return default;
            }
            return await result.Rows.ToListAsync<T>();
        }

        public async Task<IReadOnlyList<T>> ListAsync(BaseFilter<T> filter, CancellationToken token = default)
        {
            // TODO filter belum

            var colName = typeof(T).Name.ToLower();
            var bucketName = _couchbaseRepository.DefaultBucket.Name;
            var tableName = $"`{bucketName}`.{CB_COLLECTION}.{colName}";
            var sql = $"SELECT g.* FROM {tableName} AS g";
            var result = await _couchbaseRepository.Cluster.QueryAsync<T>(sql);
            if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
            {
                return default;
            }
            return await result.Rows.ToListAsync<T>();
        }

        public async Task DeleteAsync(BaseFilter<T> filter, CancellationToken token = default)
        {
            // TODO search dulu by filter, delete semua yang ter pilih
            throw new NotImplementedException();
        }

        public async Task<int> CountAsync(BaseFilter<T> filter, CancellationToken token = default)
        {
            // TODO filter belum

            var colName = typeof(T).Name.ToLower();
            var bucketName = _couchbaseRepository.DefaultBucket.Name;
            var tableName = $"`{bucketName}`.{CB_COLLECTION}.{colName}";
            var sql = $"SELECT COUNT(*) as rowcount FROM {tableName}";
            var result = await _couchbaseRepository.Cluster.QueryAsync<dynamic>(sql);
            if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
            {
                return default;
            }
            var row = await result.Rows.FirstOrDefaultAsync<dynamic>();
            return row.rowcount;
        }

        public async Task<T> FirstAsync(BaseFilter<T> filter, CancellationToken token = default)
        {
            // TODO filter belum

            var colName = typeof(T).Name.ToLower();
            var bucketName = _couchbaseRepository.DefaultBucket.Name;
            var tableName = $"`{bucketName}`.{CB_COLLECTION}.{colName}";
            var sql = $"SELECT g.* FROM {tableName} AS g LIMIT 1";
            var result = await _couchbaseRepository.Cluster.QueryAsync<T>(sql);
            if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
            {
                return default;
            }
            return await result.Rows.FirstAsync<T>();
        }

        public async Task<T> FirstOrDefaultAsync(BaseFilter<T> filter, CancellationToken token = default)
        {
            // TODO filter belum

            var colName = typeof(T).Name.ToLower();
            var bucketName = _couchbaseRepository.DefaultBucket.Name;
            var tableName = $"`{bucketName}`.{CB_COLLECTION}.{colName}";
            var sql = $"SELECT g.* FROM {tableName} AS g LIMIT 1";
            var result = await _couchbaseRepository.Cluster.QueryAsync<T>(sql);
            if (result.MetaData.Status != Couchbase.Query.QueryStatus.Success)
            {
                return default;
            }
            return await result.Rows.FirstOrDefaultAsync<T>();
        }
    }
}
