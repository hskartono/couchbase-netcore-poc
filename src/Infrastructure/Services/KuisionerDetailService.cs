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
	public class KuisionerDetailService : AsyncBaseService<KuisionerDetail>, IKuisionerDetailService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public KuisionerDetailService(
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
		public async Task<KuisionerDetail> AddAsync(KuisionerDetail entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);



			await _unitOfWork.KuisionerDetailRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<KuisionerDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerDetailRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(KuisionerDetail entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.KuisionerDetailRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<KuisionerDetail> FirstAsync(ISpecification<KuisionerDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerDetailRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<KuisionerDetail> FirstOrDefaultAsync(ISpecification<KuisionerDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerDetailRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<KuisionerDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<KuisionerDetail> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new KuisionerDetailFilterSpecification(id, true);
			var kuisionerDetail = await _unitOfWork.KuisionerDetailRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (kuisionerDetail == null || includeChilds == false)
				return kuisionerDetail;



			return kuisionerDetail;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<KuisionerDetail>> ListAllAsync(List<SortingInformation<KuisionerDetail>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerDetailRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<KuisionerDetail>> ListAsync(
			ISpecification<KuisionerDetail> spec, 
			List<SortingInformation<KuisionerDetail>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var kuisionerDetails = await _unitOfWork.KuisionerDetailRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && kuisionerDetails?.Count > 0)
			{
				var results = new List<KuisionerDetail>(kuisionerDetails);
				var kuisionerDetailIds = kuisionerDetails.Select(e => e.Id).ToList();



				return results;
			}

			return kuisionerDetails;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(KuisionerDetail entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.KuisionerDetailRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			
			var oldEntity = await _unitOfWork.KuisionerDetailRepository.GetByIdAsync(entity.Id, cancellationToken);
			if (oldEntity == null)
			{
			AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
			return false;
			}


			// update & commit
			//await _unitOfWork.KuisionerDetailRepository.UpdateAsync(oldEntity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}


		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(KuisionerDetail entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");



			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(KuisionerDetail entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(KuisionerDetail entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(KuisionerDetail entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("kuisioner_detail.html");
			string htmlContent = File.ReadAllText(templateFile);

			htmlContent = MapTemplateValue(htmlContent, entity);

			// prepare destination pdf
			string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
			string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
			ConverterProperties converterProperties = new ConverterProperties();
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

			var items = await this.ListAsync(new KuisionerDetailFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("kuisioner_detail.html");
			string htmlContent = File.ReadAllText(templateFile);
			string result = "";

			foreach (var item in items)
			{
				result += MapTemplateValue(htmlContent, item);
			}

			// prepare destination pdf
			string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
			string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
			ConverterProperties converterProperties = new ConverterProperties();
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
			var downloadProcess = new DownloadProcess("kuisioner_detail") { StartTime = DateTime.Now };
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
		private string MapTemplateValue(string htmlContent, KuisionerDetail entity)
		{
			Dictionary<string, object> mapper = new Dictionary<string, object>()
				{
					{"Id",""},
					{"KontenSoal",""},
					{"Pilihan1",""},
					{"PIlihan2",""},
					{"Pilihan3",""},
					{"KunciJawaban",""},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["Kuisioner"] = entity.Kuisioner.Judul;
				mapper["Soal"] = entity.Soal.Konten;
				mapper["KontenSoal"] = entity.KontenSoal;
				mapper["Pilihan1"] = entity.Pilihan1;
				mapper["PIlihan2"] = entity.PIlihan2;
				mapper["Pilihan3"] = entity.Pilihan3;
				mapper["KunciJawaban"] = entity.KunciJawaban?.ToString("#,#0");

			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			int? id = null, List<int> kuisioners = null, List<string> soals = null, List<string> kontenSoals = null, List<string> pilihan1s = null, List<string> pIlihan2s = null, List<string> pilihan3s = null, List<int> kunciJawabans = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("kuisioner_detail") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, kuisioners, soals, kontenSoals, pilihan1s, pIlihan2s, pilihan3s, kunciJawabans,
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
			int? id = null, List<int> kuisioners = null, List<string> soals = null, List<string> kontenSoals = null, List<string> pilihan1s = null, List<string> pIlihan2s = null, List<string> pilihan3s = null, List<int> kunciJawabans = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				KuisionerDetailFilterSpecification filterSpec = null;
				if (id.HasValue)
					filterSpec = new KuisionerDetailFilterSpecification(id.Value);
				else
					filterSpec = new KuisionerDetailFilterSpecification(exact)
					{

						Id = id, 
						KuisionerIds = kuisioners, 
						SoalIds = soals, 
						KontenSoals = kontenSoals, 
						Pilihan1s = pilihan1s, 
						PIlihan2s = pIlihan2s, 
						Pilihan3s = pilihan3s, 
						KunciJawabans = kunciJawabans
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<KuisionerDetail>(excelFilename, "kuisionerDetail.json", results) == false)
				{
					if (refId.HasValue)
						await _downloadProcessService.FailedToGenerate(refId.Value, "Failed to generate excel file");
					return "";
				}

				// update database information (if needed)
				if (refId.HasValue)
				{
					excelFilename = Path.GetFileName(excelFilename);
					await _downloadProcessService.SuccessfullyGenerated(refId.Value, excelFilename);
				}

				return excelFilename;
			}
			catch (Exception ex)
			{
				if (refId.HasValue)
					await _downloadProcessService.FailedToGenerate(refId.Value, ex.Message);

				throw;
			}
		}
		#endregion

		#region appgen: upload excel
		public async Task<List<KuisionerDetail>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<KuisionerDetail>(tempExcelFile, "kuisionerDetail.json");
			if (result == null)
			{
				AddError("Format template excel tidak dikenali. Silahkan download template dari menu download.");
				return null;
			}

			SetUploadDraftFlags(result);

			await RunMasterDataValidation(result, cancellationToken);

			foreach (var item in result)
			{
				var id = item.Id;
				if (id > 0)
					await _unitOfWork.KuisionerDetailRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.KuisionerDetailRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync();

			return result;
		}

		private async Task RunMasterDataValidation(List<KuisionerDetail> result, CancellationToken cancellationToken)
		{
			var excelKuisionerIds1 = result.Where(s => s.KuisionerId.HasValue).Select(s => s.KuisionerId.Value).ToList();
			var Kuisioners = await _unitOfWork.KuisionerRepository.ListAsync(new KuisionerFilterSpecification(excelKuisionerIds1), null, cancellationToken);
			var KuisionerIds = Kuisioners.Select(e => e.Id);
			var excelSoalIds1 = result.Where(s => !string.IsNullOrEmpty(s.SoalId)).Select(s => s.SoalId).ToList();
			var Soals = await _unitOfWork.SoalRepository.ListAsync(new SoalFilterSpecification(excelSoalIds1), null, cancellationToken);
			var SoalIds = Soals.Select(e => e.Id);

		}

		private void SetUploadDraftFlags(List<KuisionerDetail> result)
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
			var spec = new KuisionerDetailFilterSpecification()
			{
				RecordEditedBy = _userName,
				DraftFromUpload = true,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification(true);
			var draftDatas = await ListAsync(spec, null, true, cancellationToken);
			int rowNum = 1;
			foreach (var item in draftDatas)
			{
				ValidateOnInsert(item, rowNum);
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
		public async Task<bool> ProcessUploadedFile(IEnumerable<KuisionerDetail> kuisionerDetails, CancellationToken cancellationToken = default)
		{
			if (kuisionerDetails == null)
				throw new ArgumentNullException(nameof(kuisionerDetails));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in kuisionerDetails)
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
			var spec = new KuisionerDetailFilterSpecification()
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
				var ws = package.Workbook.Worksheets.Add("KuisionerDetail");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "Kuisioner";
				ws.Cells[1, 4].Value = "Soal";
				ws.Cells[1, 5].Value = "Konten Soal";
				ws.Cells[1, 6].Value = "Pilihan 1";
				ws.Cells[1, 7].Value = "PIlihan 2";
				ws.Cells[1, 8].Value = "Pilihan 3";
				ws.Cells[1, 9].Value = "Kunci Jawaban";
				ws.Cells[1, 10].Value = "Status";
				ws.Cells[1, 11].Value = "Message";



				int row = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.KontenSoal;
					ws.Cells[row, 4].Value = item.Pilihan1;
					ws.Cells[row, 5].Value = item.PIlihan2;
					ws.Cells[row, 6].Value = item.Pilihan3;
					ws.Cells[row, 7].Value = item.KunciJawaban;
					ws.Cells[row, 8].Value = item.UploadValidationStatus;
					ws.Cells[row, 9].Value = item.UploadValidationMessage;


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
		public async Task<KuisionerDetail> CreateDraft(KuisionerDetail entity, CancellationToken cancellation)
		{
			entity.Kuisioner = null;
			entity.Soal = null;

			entity.IsDraftRecord = 1;
			entity.MainRecordId = null;
			entity.RecordEditedBy = _userName;
			entity.RecordActionDate = DateTime.Now;

			AssignCreatorAndCompany(entity);

			await _unitOfWork.KuisionerDetailRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<KuisionerDetail> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new KuisionerDetailFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Kuisioner Detail dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new KuisionerDetailFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.KuisionerDetailRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.KuisionerDetailRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Kuisioner Detail");
				return null;
			}

			return await _unitOfWork.KuisionerDetailRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(KuisionerDetail kuisionerDetail, CancellationToken cancellationToken)
		{
			var id = kuisionerDetail.Id;
			var originalValue = await _unitOfWork.KuisionerDetailRepository.FirstOrDefaultAsync(
				new KuisionerDetailFilterSpecification(id));

			if(originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(kuisionerDetail.SoalId)) originalValue.SoalId = kuisionerDetail.SoalId;
			if (!string.IsNullOrEmpty(kuisionerDetail.KontenSoal)) originalValue.KontenSoal = kuisionerDetail.KontenSoal;
			if (!string.IsNullOrEmpty(kuisionerDetail.Pilihan1)) originalValue.Pilihan1 = kuisionerDetail.Pilihan1;
			if (!string.IsNullOrEmpty(kuisionerDetail.PIlihan2)) originalValue.PIlihan2 = kuisionerDetail.PIlihan2;
			if (!string.IsNullOrEmpty(kuisionerDetail.Pilihan3)) originalValue.Pilihan3 = kuisionerDetail.Pilihan3;
			if (kuisionerDetail.KunciJawaban.HasValue && kuisionerDetail.KunciJawaban > 0) originalValue.KunciJawaban = kuisionerDetail.KunciJawaban.Value;


			// pastikan data belongsTo & hasMany tidak ikut
			kuisionerDetail.Kuisioner = null;
			kuisionerDetail.Soal = null;


			AssignUpdater(originalValue);
			await _unitOfWork.KuisionerDetailRepository.UpdateAsync(originalValue, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: commit draft
		public async Task<int> CommitDraft(int id, CancellationToken cancellationToken)
		{
			int resultId = 0;
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return 0;

			KuisionerDetail destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.Kuisioner = recoveryRecord.Kuisioner;
				destinationRecord.Kuisioner = null;
				destinationRecord.Soal = recoveryRecord.Soal;
				destinationRecord.Soal = null;
				destinationRecord.KontenSoal = recoveryRecord.KontenSoal;
				destinationRecord.Pilihan1 = recoveryRecord.Pilihan1;
				destinationRecord.PIlihan2 = recoveryRecord.PIlihan2;
				destinationRecord.Pilihan3 = recoveryRecord.Pilihan3;
				destinationRecord.KunciJawaban = recoveryRecord.KunciJawaban;


				await _unitOfWork.KuisionerDetailRepository.UpdateAsync(destinationRecord, cancellationToken);
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
			await _unitOfWork.KuisionerDetailRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

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
			await _unitOfWork.KuisionerDetailRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new KuisionerDetailFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new List<DocumentDraft>();
			var datas = await _unitOfWork.KuisionerDetailRepository.ListAsync(spec, null, cancellationToken);
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
			var spec = new KuisionerDetailFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.KuisionerDetailRepository.ListAsync(spec, null, cancellationToken);
			if (datas == null) return null;

			List<string> users = new List<string>();
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

		#region appgen: add draft async
		public async Task<List<KuisionerDetail>> AddDraftAsync(List<KuisionerDetail> kuisionerDetails, CancellationToken cancellationToken)
		{
			if (kuisionerDetails == null || kuisionerDetails.Count <= 0)
				throw new ArgumentNullException(nameof(kuisionerDetails));

			foreach(var item in kuisionerDetails)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.DraftFromUpload = false;
				item.RecordEditedBy = _userName;
				item.RecordActionDate = DateTime.Now;
				await _unitOfWork.KuisionerDetailRepository.AddAsync(item, cancellationToken);
			}

			await _unitOfWork.CommitAsync();
			return kuisionerDetails;
		}
		#endregion

		#region appgen: replace draft async
		public async Task<List<KuisionerDetail>> ReplaceDraftAsync(List<KuisionerDetail> kuisionerDetails, CancellationToken cancellationToken)
		{
			if (kuisionerDetails == null || kuisionerDetails.Count <= 0)
				throw new ArgumentNullException(nameof(kuisionerDetails));

			// hapus seluruh data sesuai filter
			int parentId = (kuisionerDetails[0].KuisionerId.HasValue) ? kuisionerDetails[0].KuisionerId.Value : 0;
			var spec = new KuisionerDetailFilterSpecification()
			{
				KuisionerIds = new List<int>() { parentId }
			}.BuildSpecification();
			_unitOfWork.KuisionerDetailRepository.DeleteAsync(spec, cancellationToken);

			// insert data baru
			foreach (var item in kuisionerDetails)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.DraftFromUpload = false;
				item.RecordEditedBy = _userName;
				item.RecordActionDate = DateTime.Now;
				await _unitOfWork.KuisionerDetailRepository.AddAsync(item, cancellationToken);
			}

			await _unitOfWork.CommitAsync();

			return kuisionerDetails;
		}
		#endregion

		#endregion

	}
}
