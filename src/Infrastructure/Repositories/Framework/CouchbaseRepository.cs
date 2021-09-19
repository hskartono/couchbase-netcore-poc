using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Couchbase;
using Couchbase.KeyValue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
    public static class StringExtension
	{
        public static string DefaultIfEmpty(this string str, string defaultValue)
            => string.IsNullOrWhiteSpace(str) ? defaultValue : str;
    }

    public class CouchbaseRepository : ICouchbaseRepository
    {
        public CouchbaseRepository(IConfigurationSection configSection)
        {
            var CB_HOST = configSection["Host"];
            var CB_USER = configSection["Username"];
            var CB_PSWD = configSection["Password"];
            var CB_BUCKET = configSection["Bucket"];

            try
            {
                var task = Task.Run(async () =>
                {
                    var options = new ClusterOptions();
                    options.UserName = CB_USER;
                    options.Password = CB_PSWD;

                    var cluster = await Couchbase.Cluster.ConnectAsync(
                        $"couchbase://{CB_HOST}", options);

                    Cluster = cluster;
                    DefaultBucket = await cluster.BucketAsync(CB_BUCKET);
                    //var jejakPendapatScope = await TravelSampleBucket.ScopeAsync("jejak_pendapat");
                    //SoalCollection = await jejakPendapatScope.CollectionAsync("soal");
                });
                task.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) => throw x);
            }
        }
        public ICluster Cluster { get; private set; }

        public IBucket DefaultBucket { get; private set; }

        //public ICouchbaseCollection SoalCollection { get; private set; }

        public async Task<ICouchbaseCollection> TenantCollection(string scope, string collection)
        {
            var tenantScope = await DefaultBucket.ScopeAsync(scope);
            var tenantCollection = await tenantScope.CollectionAsync(collection);
            return tenantCollection;
        }
    }
}