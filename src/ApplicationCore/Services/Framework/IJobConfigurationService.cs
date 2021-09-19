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
	public interface IJobConfigurationService : IAsyncBaseService<JobConfiguration>
	{
        #region appgen: crud operations

        Task<JobConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JobConfiguration>> ListAllAsync(List<SortingInformation<JobConfiguration>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JobConfiguration>> ListAsync(
            ISpecification<JobConfiguration> spec, 
            List<SortingInformation<JobConfiguration>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<JobConfiguration> AddAsync(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default);
        Task<JobConfiguration> FirstAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default);
        Task<JobConfiguration> FirstOrDefaultAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<string> interfaceNames = null, List<string> jobNames = null, List<string> jobDescriptions = null, List<bool> isStoredProcedures = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<string> interfaceNames = null, List<string> jobNames = null, List<string> jobDescriptions = null, List<bool> isStoredProcedures = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);

        Task<List<JobConfiguration>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<JobConfiguration> jobConfigurations, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<JobConfiguration> CreateDraft(CancellationToken cancellation);
		Task<JobConfiguration> CreateEditDraft(int id, CancellationToken cancellation);
		Task<bool> PatchDraft(JobConfiguration jobConfiguration, CancellationToken cancellation);
		Task<int> CommitDraft(int id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


		#endregion
	}
}
