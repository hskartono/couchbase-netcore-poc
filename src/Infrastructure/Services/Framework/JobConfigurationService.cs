using Ardalis.Specification;
using Hangfire;
using iText.Html2pdf;
using OfficeOpenXml;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using AppCoreApi.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace AppCoreApi.Infrastructure.Services
{
	public class JobConfigurationService : AsyncBaseService<JobConfiguration>, IJobConfigurationService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public JobConfigurationService(
			IUnitOfWork unitOfWork,
			IDownloadProcessService downloadProcessService,
			IUriComposer uriComposer) : base(unitOfWork)
		{
			_downloadProcessService = downloadProcessService;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operations

		#region appgen: add
		public async Task<JobConfiguration> AddAsync(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);



			await _unitOfWork.JobConfigurationRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.JobConfigurationRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<JobConfiguration> FirstAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<JobConfiguration> FirstOrDefaultAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<JobConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<JobConfiguration> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new JobConfigurationFilterSpecification(id, true);
			var jobConfiguration = await _unitOfWork.JobConfigurationRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (jobConfiguration == null || includeChilds == false)
				return jobConfiguration;



			return jobConfiguration;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<JobConfiguration>> ListAllAsync(List<SortingInformation<JobConfiguration>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<JobConfiguration>> ListAsync(
			ISpecification<JobConfiguration> spec, 
			List<SortingInformation<JobConfiguration>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var jobConfigurations = await _unitOfWork.JobConfigurationRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && jobConfigurations?.Count > 0)
			{
				var results = new List<JobConfiguration>(jobConfigurations);
				var jobConfigurationIds = jobConfigurations.Select(e => e.Id).ToList();



				return results;
			}

			return jobConfigurations;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.JobConfigurationRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			


			// update & commit
			//await _unitOfWork.JobConfigurationRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}


		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(JobConfiguration entity)
		{
            if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");

			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(JobConfiguration entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(JobConfiguration entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("job_configuration.html");
			string htmlContent = File.ReadAllText(templateFile);

			htmlContent = MapTemplateValue(htmlContent, entity);

			// prepare destination pdf
			string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
			string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
			ConverterProperties converterProperties = new();
			HtmlConverter.ConvertToPdf(htmlContent, new FileStream(fullPdfFileName, FileMode.Create), converterProperties);

			return fullPdfFileName;
		}
		#endregion

		#region appgen: generate pdf multipage
		public async Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default)
		{
			if (ids == null)
				throw new ArgumentNullException(nameof(ids));

			cancellationToken.ThrowIfCancellationRequested();

			var items = await this.ListAsync(new JobConfigurationFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("job_configuration.html");
			string htmlContent = File.ReadAllText(templateFile);
			string result = "";

			foreach (var item in items)
			{
				result += MapTemplateValue(htmlContent, item);
			}

			// prepare destination pdf
			string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
			string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
			ConverterProperties converterProperties = new();
			HtmlConverter.ConvertToPdf(result, new FileStream(fullPdfFileName, FileMode.Create), converterProperties);

			if (refId.HasValue && refId.Value > 0)
			{
				await _downloadProcessService.SuccessfullyGenerated(refId.Value, pdfFileName, cancellationToken);
			}

			return fullPdfFileName;
		}
		#endregion

		#region appgen: generate pdf background process
		public async Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("job_configuration") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GeneratePdfMultiPage(ids, result.Id, cancellationToken));

			// update background job id
			result.JobId = jobId;
			await _downloadProcessService.UpdateAsync(result, cancellationToken);

			// return background job id
			return jobId;
		}
		#endregion

		#region appgen: mapp templates
		private string MapTemplateValue(string htmlContent, JobConfiguration entity)
		{
			Dictionary<string, object> mapper = new()
				{
					{"Id",""},
					{"InterfaceName",""},
					{"JobName",""},
					{"JobDescription",""},
					{"IsStoredProcedure",""},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["InterfaceName"] = entity.InterfaceName;
				mapper["JobName"] = entity.JobName;
				mapper["JobDescription"] = entity.JobDescription;
				mapper["IsStoredProcedure"] = (entity.IsStoredProcedure.HasValue && entity.IsStoredProcedure.Value) ? "Yes" : "No";

			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			int? id = null, List<string> interfaceNames = null, List<string> jobNames = null, List<string> jobDescriptions = null, List<bool> isStoredProcedures = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("job_configuration") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, interfaceNames, jobNames, jobDescriptions, isStoredProcedures,
				exact,
				cancellationToken));

			result.JobId = jobId;
			await _downloadProcessService.UpdateAsync(result, cancellationToken);

			// return background job id
			return jobId;
		}
		#endregion

		#region appgen: generate excel process
		public async Task<string> GenerateExcel(string excelFilename, int? refId = null, 
			int? id = null, List<string> interfaceNames = null, List<string> jobNames = null, List<string> jobDescriptions = null, List<bool> isStoredProcedures = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				JobConfigurationFilterSpecification filterSpec = null;
				if (id.HasValue)
					filterSpec = new JobConfigurationFilterSpecification(id.Value);
				else
					filterSpec = new JobConfigurationFilterSpecification(exact)
					{

						Id = id, 
						InterfaceNames = interfaceNames, 
						JobNames = jobNames, 
						JobDescriptions = jobDescriptions, 
						IsStoredProcedures = isStoredProcedures
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<JobConfiguration>(excelFilename, "jobConfiguration.json", results) == false)
				{
					if (refId.HasValue)
						await _downloadProcessService.FailedToGenerate(refId.Value, "Failed to generate excel file", cancellationToken);
					return "";
				}

				// update database information (if needed)
				if (refId.HasValue)
				{
					excelFilename = Path.GetFileName(excelFilename);
					await _downloadProcessService.SuccessfullyGenerated(refId.Value, excelFilename, cancellationToken);
				}

				return excelFilename;
			}
			catch (Exception ex)
			{
				if (refId.HasValue)
					await _downloadProcessService.FailedToGenerate(refId.Value, ex.Message, cancellationToken);

				throw;
			}
		}
		#endregion

		#region appgen: upload excel
		public async Task<List<JobConfiguration>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<JobConfiguration>(tempExcelFile, "jobConfiguration.json");
			if (result == null)
			{
				AddError("Format template excel tidak dikenali. Silahkan download template dari menu download.");
				return null;
			}

			SetUploadDraftFlags(result);

			foreach (var item in result)
			{
				var id = item.Id;
				if (id > 0)
					await _unitOfWork.JobConfigurationRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.JobConfigurationRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync(cancellationToken);

			return result;
		}

		private void SetUploadDraftFlags(List<JobConfiguration> result)
		{
			foreach (var item in result)
			{
				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;

				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;

			}
		}
		#endregion

		#region appgen: commit uploaded excel fiel
		public async Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default)
		{
			var spec = new JobConfigurationFilterSpecification()
			{
				RecordEditedBy = _userName,
				DraftFromUpload = true,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification(true);
			var draftDatas = await ListAsync(spec, null, true, cancellationToken);
			int rowNum = 1;
			foreach (var item in draftDatas)
			{
				ValidateOnInsert(item);
				rowNum++;
			}

			if (!ServiceState)
				return false;

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in draftDatas)
				{
					var id = await CommitDraft(item.Id, cancellationToken);
					if(id <= 0)
					{
						AddError("Terjadi kesalahan ketika menyimpan data.");
						return false;
					}
				}

				scope.Complete();
			}

			return true;
		}
		#endregion

		#region appgen: process uploaded file
		public async Task<bool> ProcessUploadedFile(IEnumerable<JobConfiguration> jobConfigurations, CancellationToken cancellationToken = default)
		{
			if (jobConfigurations == null)
				throw new ArgumentNullException(nameof(jobConfigurations));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in jobConfigurations)
				{
					var id = item.Id;
					if (id <= 0)
					{
						var result = await this.AddAsync(item, cancellationToken);
						if (result == null)
							return false;
					}
					else
					{
						var result = await this.UpdateAsync(item, cancellationToken);
						if (result == false)
							return false;
					}
				}

				scope.Complete();
			}

			return true;
		}
		#endregion

		#region appgen: generate upload log
		public async Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default)
		{
			var spec = new JobConfigurationFilterSpecification()
			{
				RecordEditedBy = _userName,
				DraftFromUpload = true,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification(true);
			var draftDatas = await ListAsync(spec, null, true, cancellationToken);

			var tempExcelFile = Path.GetTempFileName();
			InitEPPlusLicense();
			using (var package = new ExcelPackage())
			{
				var ws = package.Workbook.Worksheets.Add("JobConfiguration");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "Interface Name";
				ws.Cells[1, 4].Value = "Job Name";
				ws.Cells[1, 5].Value = "Job Description";
				ws.Cells[1, 6].Value = "Is Stored Procedure";
				ws.Cells[1, 7].Value = "Status";
				ws.Cells[1, 8].Value = "Message";



				int row = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.InterfaceName;
					ws.Cells[row, 4].Value = item.JobName;
					ws.Cells[row, 5].Value = item.JobDescription;
					ws.Cells[row, 6].Value = item.IsStoredProcedure;
					ws.Cells[row, 7].Value = item.UploadValidationStatus;
					ws.Cells[row, 8].Value = item.UploadValidationMessage;


					row++;
					pk++;
				}

				package.SaveAs(new FileInfo(tempExcelFile));
			}

			return tempExcelFile;
		}
		#endregion

		#endregion

		#region Recovery Record Service

		#region appgen: create draft
		public async Task<JobConfiguration> CreateDraft(CancellationToken cancellation)
		{
			var spec = new JobConfigurationFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordIdIsNull = true,
				RecordEditedBy = _userName
			}.BuildSpecification();
			var count = await _unitOfWork.JobConfigurationRepository.CountAsync(spec, cancellation);
			if (count > 0)
				return await _unitOfWork.JobConfigurationRepository.FirstOrDefaultAsync(spec, cancellation);

            var entity = new JobConfiguration
            {
                IsDraftRecord = 1,
                MainRecordId = null,
                RecordEditedBy = _userName,
                RecordActionDate = DateTime.Now
            };

            AssignCreatorAndCompany(entity);

			await _unitOfWork.JobConfigurationRepository.AddAsync(entity, cancellation);
			await _unitOfWork.CommitAsync(cancellation);
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<JobConfiguration> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new JobConfigurationFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Job Configuration dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new JobConfigurationFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.JobConfigurationRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.JobConfigurationRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Job Configuration");
				return null;
			}

			return await _unitOfWork.JobConfigurationRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(JobConfiguration jobConfiguration, CancellationToken cancellationToken)
		{
			var id = jobConfiguration.Id;
			var originalValue = await _unitOfWork.JobConfigurationRepository.FirstOrDefaultAsync(new JobConfigurationFilterSpecification(id), cancellationToken);

			if(originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(jobConfiguration.InterfaceName)) originalValue.InterfaceName = jobConfiguration.InterfaceName;
			if (!string.IsNullOrEmpty(jobConfiguration.JobName)) originalValue.JobName = jobConfiguration.JobName;
			if (!string.IsNullOrEmpty(jobConfiguration.JobDescription)) originalValue.JobDescription = jobConfiguration.JobDescription;
			if (jobConfiguration.IsStoredProcedure.HasValue) originalValue.IsStoredProcedure = jobConfiguration.IsStoredProcedure.Value;


			// pastikan data belongsTo & hasMany tidak ikut


			AssignUpdater(originalValue);
			await _unitOfWork.JobConfigurationRepository.UpdateAsync(originalValue, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}
		#endregion

		#region appgen: commit draft
		public async Task<int> CommitDraft(int id, CancellationToken cancellationToken)
		{
			int resultId = 0;
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return 0;

			JobConfiguration destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.InterfaceName = recoveryRecord.InterfaceName;
				destinationRecord.JobName = recoveryRecord.JobName;
				destinationRecord.JobDescription = recoveryRecord.JobDescription;
				destinationRecord.IsStoredProcedure = recoveryRecord.IsStoredProcedure;


				await _unitOfWork.JobConfigurationRepository.UpdateAsync(destinationRecord, cancellationToken);
				resultId = destinationRecord.Id;
			}

			// header recovery
			int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
			if (destinationRecord != null)
				draftStatus = (int)BaseEntity.DraftStatus.Saved;
			
			recoveryRecord.IsDraftRecord = draftStatus;
			recoveryRecord.RecordActionDate = DateTime.Now;
			recoveryRecord.DraftFromUpload = false;



			// save ke database
			await _unitOfWork.JobConfigurationRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			return (destinationRecord == null) ? recoveryRecord.Id : resultId;
		}
		

		#endregion

		#region appgen: discard draft
		public async Task<bool> DiscardDraft(int id, CancellationToken cancellationToken)
		{
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return false;

			// header
			recoveryRecord.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
			recoveryRecord.RecordActionDate = DateTime.Now;


			// save ke database
			await _unitOfWork.JobConfigurationRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new JobConfigurationFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new();
			var datas = await _unitOfWork.JobConfigurationRepository.ListAsync(spec, null, cancellationToken);
			if (datas == null) return documentDrafts;

			foreach (var item in datas)
				documentDrafts.Add(
					new DocumentDraft($"Ada dokumen yang di edit terakhir pada {item.RecordActionDate?.ToString("dd-MMM-yyyy")} Jam {item.RecordActionDate?.ToString("hh:mm")} dan belum di simpan.", item.Id));

			return documentDrafts;
		}
		#endregion

		#region appgen: get current editor
		public async Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken)
		{
			var spec = new JobConfigurationFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.JobConfigurationRepository.ListAsync(spec, null, cancellationToken);
			if (datas == null) return null;

			List<string> users = new();
			foreach (var item in datas)
				users.Add(item.RecordEditedBy);

			var editorUsers = await _unitOfWork.UserInfoRepository.ListAsync(new UserInfoFilterSpecification(users), null, cancellationToken);
			users.Clear();
			if (editorUsers == null) return users;

			foreach (var item in editorUsers)
				users.Add(item.FullName);

			return users;
		}
		#endregion



		#endregion

	}
}
