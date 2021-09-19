using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
    public interface IRoleManagementService : IAsyncBaseService<RoleManagement>
    {
        #region appgen: crud operations

        Task<RoleManagement> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RoleManagement>> ListAllAsync(List<SortingInformation<RoleManagement>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RoleManagement>> ListAsync(
            ISpecification<RoleManagement> spec,
            List<SortingInformation<RoleManagement>> sorting,
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<RoleManagement> AddAsync(RoleManagement entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(RoleManagement entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(RoleManagement entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<RoleManagement> spec, CancellationToken cancellationToken = default);
        Task<RoleManagement> FirstAsync(ISpecification<RoleManagement> spec, CancellationToken cancellationToken = default);
        Task<RoleManagement> FirstOrDefaultAsync(ISpecification<RoleManagement> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(RoleManagement entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
            int? id = null, List<string> names = null, List<string> descriptions = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null,
            int? id = null, List<string> names = null, List<string> descriptions = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default);

        Task<List<RoleManagement>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<RoleManagement> roleManagements, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

        Task<RoleManagement> CreateDraft(CancellationToken cancellation);
        Task<RoleManagement> CreateEditDraft(int id, CancellationToken cancellation);
        Task<bool> PatchDraft(RoleManagement roleManagement, CancellationToken cancellation);
        Task<int> CommitDraft(int id, CancellationToken cancellationToken);
        Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
        Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
        Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


        #endregion
    }
}
