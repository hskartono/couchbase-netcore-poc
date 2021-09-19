using Ardalis.Specification;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCoreApi.ApplicationCore.Specifications.Filters;

namespace AppCoreApi.ApplicationCore.Services
{
	public interface ISoalService : IAsyncBaseService<Soal>
	{
        #region appgen: crud operations

        Task<Soal> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Soal>> ListAllAsync(List<SortingInformation<Soal>> sorting, CancellationToken cancellationToken = default);

        [Obsolete("Use ListAsync(SoalFilter filter, CancellationToken token = default) instead")]
        Task<IReadOnlyList<Soal>> ListAsync(
            ISpecification<Soal> spec, 
            List<SortingInformation<Soal>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Soal>> ListAsync(
            SoalFilter filter,
            CancellationToken token = default);

        Task<Soal> AddAsync(Soal entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Soal entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Soal entity, CancellationToken cancellationToken = default);

        [Obsolete("Use CountAsync(SoalFilter filter, CancellationToken token = default) instead")]
        Task<int> CountAsync(ISpecification<Soal> spec, CancellationToken cancellationToken = default);
        [Obsolete("Use FirstAsync(SoalFilter filter, CancellationToken token = default) instaead")]
        Task<Soal> FirstAsync(ISpecification<Soal> spec, CancellationToken cancellationToken = default);
        [Obsolete("Use FirstOrDefaultAsync(SoalFilter filter, CancellationToken token = default) instead")]
        Task<Soal> FirstOrDefaultAsync(ISpecification<Soal> spec, CancellationToken cancellationToken = default);

        Task<int> CountAsync(SoalFilter filter, CancellationToken token = default);
        Task<Soal> FirstAsync(SoalFilter filter, CancellationToken token = default);
        Task<Soal> FirstOrDefaultAsync(SoalFilter filter, CancellationToken token = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(Soal entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<string> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<string> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			string id = "", List<string> kontens = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			string id = "", List<string> kontens = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);

        Task<List<Soal>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<Soal> soals, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		//Task<Soal> CreateDraft(CancellationToken cancellation);
		//Task<Soal> CreateEditDraft(string id, CancellationToken cancellation);
		//Task<bool> PatchDraft(Soal soal, CancellationToken cancellation);
		//Task<string> CommitDraft(string id, CancellationToken cancellationToken);
		//Task<bool> DiscardDraft(string id, CancellationToken cancellationToken);
		//Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		//Task<List<string>> GetCurrentEditors(string id, CancellationToken cancellationToken);


		#endregion
	}
}
