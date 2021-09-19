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
	public class KuisionerService : AsyncBaseService<Kuisioner>, IKuisionerService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public KuisionerService(
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
		public async Task<Kuisioner> AddAsync(Kuisioner entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);

			if (entity.KuisionerDetail?.Count > 0)
				foreach (var item in entity.KuisionerDetail)
				{
					AssignCreatorAndCompany(item);
					item.Kuisioner = entity;
				}


			await _unitOfWork.KuisionerRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<Kuisioner> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(Kuisioner entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.KuisionerRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<Kuisioner> FirstAsync(ISpecification<Kuisioner> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<Kuisioner> FirstOrDefaultAsync(ISpecification<Kuisioner> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<Kuisioner> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<Kuisioner> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new KuisionerFilterSpecification(id, true);
			var kuisioner = await _unitOfWork.KuisionerRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (kuisioner == null || includeChilds == false)
				return kuisioner;

			var kuisionerDetailFilter = new KuisionerDetailFilterSpecification()
			{
				KuisionerIds = new List<int>() { id },
				ShowDraftList = BaseEntity.DraftStatus.All
			}.BuildSpecification();
			var kuisionerDetails = await _unitOfWork.KuisionerDetailRepository.ListAsync(kuisionerDetailFilter, null, cancellationToken);
			kuisioner.AddRangeKuisionerDetails(kuisionerDetails.ToList());


			return kuisioner;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<Kuisioner>> ListAllAsync(List<SortingInformation<Kuisioner>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.KuisionerRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<Kuisioner>> ListAsync(
			ISpecification<Kuisioner> spec, 
			List<SortingInformation<Kuisioner>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var kuisioners = await _unitOfWork.KuisionerRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && kuisioners?.Count > 0)
			{
				var results = new List<Kuisioner>(kuisioners);
				var kuisionerIds = kuisioners.Select(e => e.Id).ToList();

			var kuisionerDetailFilter = new KuisionerDetailFilterSpecification()
			{
				KuisionerIds = kuisionerIds,
				ShowDraftList = BaseEntity.DraftStatus.All
			}.BuildSpecification();
			var kuisionerDetails = await _unitOfWork.KuisionerDetailRepository.ListAsync(kuisionerDetailFilter, null, cancellationToken);
			results.ForEach(c => c.AddRangeKuisionerDetails(
				kuisionerDetails
				.Where(e=>e.KuisionerId == c.Id).ToList()
				));


				return results;
			}

			return kuisioners;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(Kuisioner entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.KuisionerRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			
			var oldEntity = await _unitOfWork.KuisionerRepository.GetByIdAsync(entity.Id, cancellationToken);
			if (oldEntity == null)
			{
			AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
			return false;
			}
			await SmartUpdateKuisionerDetail(oldEntity, entity); 


			// update & commit
			//await _unitOfWork.KuisionerRepository.UpdateAsync(oldEntity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private async Task SmartUpdateKuisionerDetail(Kuisioner oldEntity, Kuisioner entity, CancellationToken cancellationToken = default)
		{
			List<KuisionerDetail> oldEntityToBeDeleted = new List<KuisionerDetail>();
			if (oldEntity.KuisionerDetail.Count > 0)
			{
				foreach (var item in oldEntity.KuisionerDetail)
					oldEntityToBeDeleted.Add(item);
			}

			if (entity.KuisionerDetail.Count > 0)
			{
				foreach (var item in entity.KuisionerDetail)
				{
					var hasUpdate = false;
					if (oldEntity.KuisionerDetail.Count > 0)
					{
						var data = oldEntity.KuisionerDetail.SingleOrDefault(p => p.Id == item.Id);
						if (data != null)
						{
							AssignUpdater(item);
							await _unitOfWork.KuisionerDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

							oldEntityToBeDeleted.Remove(data);
						}
					}

					if (!hasUpdate)
					{
						AssignCreatorAndCompany(item);
						oldEntity.AddOrReplaceKuisionerDetail(item);
					}
				}
			}

			if (oldEntityToBeDeleted.Count > 0)
			{
				foreach (var item in oldEntityToBeDeleted)
					oldEntity.RemoveKuisionerDetail(item);
			}
		}
		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(Kuisioner entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");



			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(Kuisioner entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(Kuisioner entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(Kuisioner entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("kuisioner.html");
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

			var items = await this.ListAsync(new KuisionerFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("kuisioner.html");
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
			var downloadProcess = new DownloadProcess("kuisioner") { StartTime = DateTime.Now };
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
		private string MapTemplateValue(string htmlContent, Kuisioner entity)
		{
			Dictionary<string, object> mapper = new Dictionary<string, object>()
				{
					{"Id",""},
					{"Judul",""},
					{"AktifDari",""},
					{"AktifSampai",""},
					{"KuisionerDetail", new List<Dictionary<string, string>>()},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["Judul"] = entity.Judul;
				mapper["AktifDari"] = entity.AktifDari?.ToString("dd-MMM-yyyy");
				mapper["AktifSampai"] = entity.AktifSampai?.ToString("dd-MMM-yyyy");
				if (entity.KuisionerDetail.Count > 0)
				{
					foreach (var item in entity.KuisionerDetail)
					{
						var kuisionerDetail = new Dictionary<string, string>()
						{
							{"KuisionerDetailId", item.Id.ToString()},
							{"KuisionerDetailKontenSoal", item.KontenSoal},
							{"KuisionerDetailPilihan1", item.Pilihan1},
							{"KuisionerDetailPIlihan2", item.PIlihan2},
							{"KuisionerDetailPilihan3", item.Pilihan3},
							{"KuisionerDetailKunciJawaban", item.KunciJawaban?.ToString("#,#0")},
						};
						((List<Dictionary<string, string>>)mapper["KuisionerDetail"]).Add(kuisionerDetail);
					}
				}


			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			int? id = null, List<string> juduls = null, DateTime? aktifDariFrom = null, DateTime? aktifDariTo = null, DateTime? aktifSampaiFrom = null, DateTime? aktifSampaiTo = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("kuisioner") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, juduls, aktifDariFrom, aktifDariTo, aktifSampaiFrom, aktifSampaiTo,
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
			int? id = null, List<string> juduls = null, DateTime? aktifDariFrom = null, DateTime? aktifDariTo = null, DateTime? aktifSampaiFrom = null, DateTime? aktifSampaiTo = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				KuisionerFilterSpecification filterSpec = null;
				if (id.HasValue)
					filterSpec = new KuisionerFilterSpecification(id.Value);
				else
					filterSpec = new KuisionerFilterSpecification(exact)
					{

						Id = id, 
						Juduls = juduls, 
						AktifDariFrom = aktifDariFrom, 
						AktifDariTo = aktifDariTo, 
						AktifSampaiFrom = aktifSampaiFrom, 
						AktifSampaiTo = aktifSampaiTo
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<Kuisioner>(excelFilename, "kuisioner.json", results) == false)
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
		public async Task<List<Kuisioner>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<Kuisioner>(tempExcelFile, "kuisioner.json");
			if (result == null)
			{
				AddError("Format template excel tidak dikenali. Silahkan download template dari menu download.");
				return null;
			}

			SetUploadDraftFlags(result);

			//await RunMasterDataValidation(result, cancellationToken);

			foreach (var item in result)
			{
				var id = item.Id;
				if (id > 0)
					await _unitOfWork.KuisionerRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.KuisionerRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync();

			return result;
		}

		//private async Task RunMasterDataValidation(List<Kuisioner> result, CancellationToken cancellationToken)
		//{

		//}

		private void SetUploadDraftFlags(List<Kuisioner> result)
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
			foreach (var subitem in item.KuisionerDetail)
			{
				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;
			}

			}
		}
		#endregion

		#region appgen: commit uploaded excel fiel
		public async Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default)
		{
			var spec = new KuisionerFilterSpecification()
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
		public async Task<bool> ProcessUploadedFile(IEnumerable<Kuisioner> kuisioners, CancellationToken cancellationToken = default)
		{
			if (kuisioners == null)
				throw new ArgumentNullException(nameof(kuisioners));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in kuisioners)
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
			var spec = new KuisionerFilterSpecification()
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
				var ws = package.Workbook.Worksheets.Add("Kuisioner");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "Judul";
				ws.Cells[1, 4].Value = "Aktif Dari";
				ws.Cells[1, 5].Value = "Aktif Sampai";
				ws.Cells[1, 6].Value = "Daftar Pertanyaan";
				ws.Cells[1, 7].Value = "Status";
				ws.Cells[1, 8].Value = "Message";

				var wsKuisionerDetail = package.Workbook.Worksheets.Add("KuisionerDetail");
				wsKuisionerDetail.Cells[1, 1].Value = "ID";
				wsKuisionerDetail.Cells[1, 2].Value = "PK";
				wsKuisionerDetail.Cells[1, 3].Value = "Kuisioner";
				wsKuisionerDetail.Cells[1, 4].Value = "Soal";
				wsKuisionerDetail.Cells[1, 5].Value = "Konten Soal";
				wsKuisionerDetail.Cells[1, 6].Value = "Pilihan 1";
				wsKuisionerDetail.Cells[1, 7].Value = "PIlihan 2";
				wsKuisionerDetail.Cells[1, 8].Value = "Pilihan 3";
				wsKuisionerDetail.Cells[1, 9].Value = "Kunci Jawaban";
				wsKuisionerDetail.Cells[1, 10].Value = "Status";
				wsKuisionerDetail.Cells[1, 11].Value = "Message";


				int row = 2;
				int rowKuisionerDetail = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.Judul;
					ws.Cells[row, 4].Value = item.AktifDari;
					ws.Cells[row, 4].Style.Numberformat.Format = "dd-MMM-yyyy";
					ws.Cells[row, 5].Value = item.AktifSampai;
					ws.Cells[row, 5].Style.Numberformat.Format = "dd-MMM-yyyy";
					ws.Cells[row, 6].Value = item.UploadValidationStatus;
					ws.Cells[row, 7].Value = item.UploadValidationMessage;
					foreach(var itemKuisionerDetail in item.KuisionerDetail)
					{
						wsKuisionerDetail.Cells[rowKuisionerDetail, 1].Value = itemKuisionerDetail.Id;
						wsKuisionerDetail.Cells[rowKuisionerDetail, 2].Value = pk;
						ws.Cells[row, 3].Value = item.UploadValidationStatus;
						ws.Cells[row, 4].Value = item.UploadValidationMessage;
					}


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
		public async Task<Kuisioner> CreateDraft(CancellationToken cancellation)
		{
			var spec = new KuisionerFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordIdIsNull = true,
				RecordEditedBy = _userName
			}.BuildSpecification();
			var count = await _unitOfWork.KuisionerRepository.CountAsync(spec, cancellation);
			if (count > 0)
				return await _unitOfWork.KuisionerRepository.FirstOrDefaultAsync(spec, cancellation);

			var entity = new Kuisioner();

			entity.IsDraftRecord = 1;
			entity.MainRecordId = null;
			entity.RecordEditedBy = _userName;
			entity.RecordActionDate = DateTime.Now;

			AssignCreatorAndCompany(entity);

			await _unitOfWork.KuisionerRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<Kuisioner> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new KuisionerFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Kuisioner dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new KuisionerFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.KuisionerRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.KuisionerRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Kuisioner");
				return null;
			}

			return await _unitOfWork.KuisionerRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(Kuisioner kuisioner, CancellationToken cancellationToken)
		{
			var id = kuisioner.Id;
			var originalValue = await _unitOfWork.KuisionerRepository.FirstOrDefaultAsync(
				new KuisionerFilterSpecification(id));

			if(originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(kuisioner.Judul)) originalValue.Judul = kuisioner.Judul;
			if (kuisioner.AktifDari.HasValue && kuisioner.AktifDari > DateTime.MinValue) originalValue.AktifDari = kuisioner.AktifDari.Value;
			if (kuisioner.AktifSampai.HasValue && kuisioner.AktifSampai > DateTime.MinValue) originalValue.AktifSampai = kuisioner.AktifSampai.Value;


			// pastikan data belongsTo & hasMany tidak ikut
			kuisioner.KuisionerDetail = null;


			AssignUpdater(originalValue);
			await _unitOfWork.KuisionerRepository.UpdateAsync(originalValue, cancellationToken);
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

			Kuisioner destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.Judul = recoveryRecord.Judul;
				destinationRecord.AktifDari = recoveryRecord.AktifDari;
				destinationRecord.AktifSampai = recoveryRecord.AktifSampai;
				this.SmartUpdateRecoveryKuisionerDetail(destinationRecord, recoveryRecord, cancellationToken);


				await _unitOfWork.KuisionerRepository.UpdateAsync(destinationRecord, cancellationToken);
				resultId = destinationRecord.Id;
			}

			// header recovery
			int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
			if (destinationRecord != null)
				draftStatus = (int)BaseEntity.DraftStatus.Saved;
			
			recoveryRecord.IsDraftRecord = draftStatus;
			recoveryRecord.RecordActionDate = DateTime.Now;
			recoveryRecord.DraftFromUpload = false;

			foreach (var item in recoveryRecord.KuisionerDetail)
			{
				item.IsDraftRecord = draftStatus;
				item.RecordActionDate = DateTime.Now;
				item.DraftFromUpload = false;
			}


			// save ke database
			await _unitOfWork.KuisionerRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return (destinationRecord == null) ? recoveryRecord.Id : resultId;
		}
		
		private void SmartUpdateRecoveryKuisionerDetail(Kuisioner destinationRecord, Kuisioner recoveryRecord, CancellationToken cancellationToken)
		{
			List<KuisionerDetail> destinationToBeDeleted = new List<KuisionerDetail>();
			if (destinationRecord.KuisionerDetail.Count > 0)
			{
				foreach (var item in destinationRecord.KuisionerDetail)
					destinationToBeDeleted.Add(item);
			}

			if (recoveryRecord.KuisionerDetail.Count > 0)
			{
				foreach (var item in recoveryRecord.KuisionerDetail)
				{
					var hasUpdate = false;
					if (destinationRecord.KuisionerDetail.Count > 0)
					{
						var data = destinationRecord.KuisionerDetail.SingleOrDefault(p => p.Id == item.MainRecordId);
						if (data != null)
						{
							data.SoalId = item.SoalId;
							data.KontenSoal = item.KontenSoal;
							data.Pilihan1 = item.Pilihan1;
							data.PIlihan2 = item.PIlihan2;
							data.Pilihan3 = item.Pilihan3;
							data.KunciJawaban = item.KunciJawaban;

							AssignUpdater(data);

							item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
							item.RecordActionDate = DateTime.Now;

							hasUpdate = true;
							destinationToBeDeleted.Remove(data);
						}
					}

					if (!hasUpdate)
					{
						//AssignCreatorAndCompany(item);
						destinationRecord.AddKuisionerDetail( item.SoalId, item.KontenSoal, item.Pilihan1, item.PIlihan2, item.Pilihan3, item.KunciJawaban);

						item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
						item.RecordActionDate = DateTime.Now;
					}
				}
			}

			if (destinationToBeDeleted.Count > 0)
			{
				foreach (var item in destinationToBeDeleted)
					destinationRecord.RemoveKuisionerDetail(item);
			}
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
			foreach (var item in recoveryRecord.KuisionerDetail)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
				item.RecordActionDate = DateTime.Now;
			}


			// save ke database
			await _unitOfWork.KuisionerRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new KuisionerFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new List<DocumentDraft>();
			var datas = await _unitOfWork.KuisionerRepository.ListAsync(spec, null, cancellationToken);
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
			var spec = new KuisionerFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.KuisionerRepository.ListAsync(spec, null, cancellationToken);
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



		#endregion

	}
}
