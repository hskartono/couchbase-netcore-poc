using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IAsyncBaseService<T> where T : CoreEntity
	{
        string UserName { get; set; }
        UserInfo UserInfo { get; set; }
        Dictionary<String, Role> Roles { get; set; }
        string FunctionId { get; set; }
        void AddError(string errorMessage);
        void ClearErrors();
        IReadOnlyList<string> Errors { get; }
        bool ServiceState { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
