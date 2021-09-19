using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface IModuleInfoService : IAsyncBaseService<ModuleInfo>
	{
        #region appgen: crud operations

        Task<ModuleInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ModuleInfo>> ListAllAsync(List<SortingInformation<ModuleInfo>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ModuleInfo>> ListAsync(
            ISpecification<ModuleInfo> spec, 
            List<SortingInformation<ModuleInfo>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<ModuleInfo> AddAsync(ModuleInfo entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(ModuleInfo entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(ModuleInfo entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<ModuleInfo> spec, CancellationToken cancellationToken = default);
        Task<ModuleInfo> FirstAsync(ISpecification<ModuleInfo> spec, CancellationToken cancellationToken = default);
        Task<ModuleInfo> FirstOrDefaultAsync(ISpecification<ModuleInfo> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(ModuleInfo entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<string> names = null, List<int> parentModuleIds = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<string> names = null, List<int> parentModuleIds = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<List<ModuleInfo>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<ModuleInfo> moduleInfos, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<ModuleInfo> CreateDraft(CancellationToken cancellation);
		Task<ModuleInfo> CreateEditDraft(int id, CancellationToken cancellation);
		Task<bool> PatchDraft(ModuleInfo moduleInfo, CancellationToken cancellation);
		Task<int> CommitDraft(int id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


		#endregion
	}
}
