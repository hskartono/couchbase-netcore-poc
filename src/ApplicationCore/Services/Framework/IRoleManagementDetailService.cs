using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
    public interface IRoleManagementDetailService : IAsyncBaseService<RoleManagementDetail>
    {
        #region appgen: crud operations

        Task<RoleManagementDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RoleManagementDetail>> ListAllAsync(List<SortingInformation<RoleManagementDetail>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RoleManagementDetail>> ListAsync(
            ISpecification<RoleManagementDetail> spec,
            List<SortingInformation<RoleManagementDetail>> sorting,
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<RoleManagementDetail> AddAsync(RoleManagementDetail entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(RoleManagementDetail entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(RoleManagementDetail entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<RoleManagementDetail> spec, CancellationToken cancellationToken = default);
        Task<RoleManagementDetail> FirstAsync(ISpecification<RoleManagementDetail> spec, CancellationToken cancellationToken = default);
        Task<RoleManagementDetail> FirstOrDefaultAsync(ISpecification<RoleManagementDetail> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(RoleManagementDetail entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
            int? id = null, List<int> roleManagements = null, List<string> functionInfos = null, List<bool> allowCreates = null, List<bool> allowReads = null, List<bool> allowUpdates = null, List<bool> allowDeletes = null, List<bool> showInMenus = null, List<bool> allowDownloads = null, List<bool> allowPrints = null, List<bool> allowUploads = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null,
            int? id = null, List<int> roleManagements = null, List<string> functionInfos = null, List<bool> allowCreates = null, List<bool> allowReads = null, List<bool> allowUpdates = null, List<bool> allowDeletes = null, List<bool> showInMenus = null, List<bool> allowDownloads = null, List<bool> allowPrints = null, List<bool> allowUploads = null,
            Dictionary<string, int> exact = null,
            CancellationToken cancellationToken = default);

        Task<List<RoleManagementDetail>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<RoleManagementDetail> roleManagementDetails, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

        Task<RoleManagementDetail> CreateDraft(RoleManagementDetail entity, CancellationToken cancellation);
        Task<bool> PatchDraft(RoleManagementDetail roleManagementDetail, CancellationToken cancellation);
        Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
        Task<List<RoleManagementDetail>> AddDraftAsync(List<RoleManagementDetail> roleManagementDetails, CancellationToken cancellationToken);
        Task<List<RoleManagementDetail>> ReplaceDraftAsync(List<RoleManagementDetail> roleManagementDetails, CancellationToken cancellationToken);


        #endregion
    }
}
