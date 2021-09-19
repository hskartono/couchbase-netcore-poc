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
	public interface IKuisionerDetailService : IAsyncBaseService<KuisionerDetail>
	{
        #region appgen: crud operations

        Task<KuisionerDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<KuisionerDetail>> ListAllAsync(List<SortingInformation<KuisionerDetail>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<KuisionerDetail>> ListAsync(
            ISpecification<KuisionerDetail> spec, 
            List<SortingInformation<KuisionerDetail>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<KuisionerDetail> AddAsync(KuisionerDetail entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(KuisionerDetail entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(KuisionerDetail entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<KuisionerDetail> spec, CancellationToken cancellationToken = default);
        Task<KuisionerDetail> FirstAsync(ISpecification<KuisionerDetail> spec, CancellationToken cancellationToken = default);
        Task<KuisionerDetail> FirstOrDefaultAsync(ISpecification<KuisionerDetail> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(KuisionerDetail entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<int> kuisioners = null, List<string> soals = null, List<string> kontenSoals = null, List<string> pilihan1s = null, List<string> pIlihan2s = null, List<string> pilihan3s = null, List<int> kunciJawabans = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<int> kuisioners = null, List<string> soals = null, List<string> kontenSoals = null, List<string> pilihan1s = null, List<string> pIlihan2s = null, List<string> pilihan3s = null, List<int> kunciJawabans = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);

        Task<List<KuisionerDetail>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<KuisionerDetail> kuisionerDetails, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<KuisionerDetail> CreateDraft(KuisionerDetail entity, CancellationToken cancellation);
		Task<bool> PatchDraft(KuisionerDetail kuisionerDetail, CancellationToken cancellation);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<KuisionerDetail>> AddDraftAsync(List<KuisionerDetail> kuisionerDetails, CancellationToken cancellationToken);
		Task<List<KuisionerDetail>> ReplaceDraftAsync(List<KuisionerDetail> kuisionerDetails, CancellationToken cancellationToken);


		#endregion
	}
}
