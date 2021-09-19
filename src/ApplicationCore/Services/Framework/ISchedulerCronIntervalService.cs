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
	public interface ISchedulerCronIntervalService : IAsyncBaseService<SchedulerCronInterval>
	{
        #region appgen: crud operations

        Task<SchedulerCronInterval> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerCronInterval>> ListAllAsync(List<SortingInformation<SchedulerCronInterval>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerCronInterval>> ListAsync(
            ISpecification<SchedulerCronInterval> spec, 
            List<SortingInformation<SchedulerCronInterval>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<SchedulerCronInterval> AddAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default);
        Task<SchedulerCronInterval> FirstAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default);
        Task<SchedulerCronInterval> FirstOrDefaultAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<string> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<string> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			string id = "", List<string> names = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			string id = "", List<string> names = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);

        Task<List<SchedulerCronInterval>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<SchedulerCronInterval> schedulerCronIntervals, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<SchedulerCronInterval> CreateDraft(CancellationToken cancellation);
		Task<SchedulerCronInterval> CreateEditDraft(string id, CancellationToken cancellation);
		Task<bool> PatchDraft(SchedulerCronInterval schedulerCronInterval, CancellationToken cancellation);
		Task<string> CommitDraft(string id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(string id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(string id, CancellationToken cancellationToken);


		#endregion
	}
}
