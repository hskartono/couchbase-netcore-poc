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
	public interface ISchedulerConfigurationService : IAsyncBaseService<SchedulerConfiguration>
	{
        #region appgen: crud operations

        Task<SchedulerConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerConfiguration>> ListAllAsync(List<SortingInformation<SchedulerConfiguration>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerConfiguration>> ListAsync(
            ISpecification<SchedulerConfiguration> spec, 
            List<SortingInformation<SchedulerConfiguration>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<SchedulerConfiguration> AddAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default);
        Task<SchedulerConfiguration> FirstAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default);
        Task<SchedulerConfiguration> FirstOrDefaultAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<string> intervalTypes = null, List<int> intervalValues = null, List<int> intervalValue2s = null, List<int> intervalValue3s = null, List<string> cronExpressions = null, List<int> jobTypes = null, List<string> recurringJobIds = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<string> intervalTypes = null, List<int> intervalValues = null, List<int> intervalValue2s = null, List<int> intervalValue3s = null, List<string> cronExpressions = null, List<int> jobTypes = null, List<string> recurringJobIds = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);

        Task<List<SchedulerConfiguration>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<SchedulerConfiguration> schedulerConfigurations, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<SchedulerConfiguration> CreateDraft(CancellationToken cancellation);
		Task<SchedulerConfiguration> CreateEditDraft(int id, CancellationToken cancellation);
		Task<bool> PatchDraft(SchedulerConfiguration schedulerConfiguration, CancellationToken cancellation);
		Task<int> CommitDraft(int id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


		#endregion
	}
}
