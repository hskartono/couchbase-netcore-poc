using Couchbase;
using Couchbase.KeyValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface ICouchbaseRepository
    {
        ICluster Cluster { get; }
        IBucket DefaultBucket { get; }
        //ICouchbaseCollection SoalCollection { get; }
        public Task<ICouchbaseCollection> TenantCollection(string scope, string collection);
    }
}
