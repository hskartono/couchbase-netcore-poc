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
	public interface IKuisionerService : IAsyncBaseService<Kuisioner>
	{
        #region appgen: crud operations

        Task<Kuisioner> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Kuisioner>> ListAllAsync(List<SortingInformation<Kuisioner>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Kuisioner>> ListAsync(
            ISpecification<Kuisioner> spec, 
            List<SortingInformation<Kuisioner>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<Kuisioner> AddAsync(Kuisioner entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Kuisioner entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Kuisioner entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Kuisioner> spec, CancellationToken cancellationToken = default);
        Task<Kuisioner> FirstAsync(ISpecification<Kuisioner> spec, CancellationToken cancellationToken = default);
        Task<Kuisioner> FirstOrDefaultAsync(ISpecification<Kuisioner> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(Kuisioner entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<string> juduls = null, DateTime? aktifDariFrom = null, DateTime? aktifDariTo = null, DateTime? aktifSampaiFrom = null, DateTime? aktifSampaiTo = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<string> juduls = null, DateTime? aktifDariFrom = null, DateTime? aktifDariTo = null, DateTime? aktifSampaiFrom = null, DateTime? aktifSampaiTo = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);

        Task<List<Kuisioner>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<Kuisioner> kuisioners, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<Kuisioner> CreateDraft(CancellationToken cancellation);
		Task<Kuisioner> CreateEditDraft(int id, CancellationToken cancellation);
		Task<bool> PatchDraft(Kuisioner kuisioner, CancellationToken cancellation);
		Task<int> CommitDraft(int id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


		#endregion
	}
}
