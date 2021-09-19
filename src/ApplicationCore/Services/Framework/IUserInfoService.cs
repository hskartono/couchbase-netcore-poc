using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IUserInfoService : IAsyncBaseService<UserInfo>
	{
        Task<UserInfo> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserInfo>> ListAllAsync(List<SortingInformation<UserInfo>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserInfo>> ListAsync(ISpecification<UserInfo> spec, List<SortingInformation<UserInfo>> sorting, CancellationToken cancellationToken = default);
        Task<UserInfo> AddAsync(UserInfo entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(UserInfo entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(UserInfo entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<UserInfo> spec, CancellationToken cancellationToken = default);
        Task<UserInfo> FirstAsync(ISpecification<UserInfo> spec, CancellationToken cancellationToken = default);
        Task<UserInfo> FirstOrDefaultAsync(ISpecification<UserInfo> spec, CancellationToken cancellationToken = default);
        Task GenerateExcel(int? refId, string firstName = "", string lastName = "", CancellationToken cancellationToken = default);
    }
}
