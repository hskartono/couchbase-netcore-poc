using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
    public interface IObjectConverter
    {
        Task<List<T>> Expand<T, K>(List<K> sourceObj, bool commitToDb = false, CancellationToken ct = default, bool isRemoveChild = true, bool isCombineChild = true);
    }
}
