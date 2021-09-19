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
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace AppCoreApi.Infrastructure.Services
{
	public class SchedulerConfigurationService : AsyncBaseService<SchedulerConfiguration>, ISchedulerConfigurationService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public SchedulerConfigurationService(
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
		public async Task<SchedulerConfiguration> AddAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);
			//entity.RecurringJobId = Guid.NewGuid().ToString();
			if (string.IsNullOrEmpty(entity.RecurringJobId))
			{
				if(entity.JobType != null)
					entity.RecurringJobId = entity.JobType.JobName;
				else
					entity.RecurringJobId = Guid.NewGuid().ToString();
			}
			entity.CronExpression = GetCronExpression(entity);

			await _unitOfWork.SchedulerConfigurationRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			await RegisterJobToHangfire(entity);
	
			return entity;
		}
		#endregion

		private async Task RegisterJobToHangfire(SchedulerConfiguration config)
		{
			if (string.IsNullOrEmpty(config.RecurringJobId)) return;
			Hangfire.RecurringJob.RemoveIfExists(config.RecurringJobId);
			if (config.IntervalTypeId == "NEVER") return;

			config.JobType = await _unitOfWork.JobConfigurationRepository.GetByIdAsync(config.JobTypeId.Value);

			string objToInstance = $"AppCoreApi.Infrastructure.Services.{config.JobType.InterfaceName}, Infrastructure";
			var objType = Type.GetType(objToInstance);
			IBackgroundService jobInstance = null;
			try
			{
				jobInstance = (IBackgroundService)Activator.CreateInstance(objType, _unitOfWork, _uriComposer);
			} catch(Exception ex)
			{
				AddError($"Gagal me-register ke background process dengan tipe: {objToInstance}. Exception: {ex.Message}");
				return;
			}

			// var obj = Activator.CreateInstance("Infrastructure", "AppCoreApi.Infrastructure.Services." + config.JobType.InterfaceName) as ObjectHandle;
			// IBackgroundService jobInstance = (IBackgroundService)obj.Unwrap();
			
			var manager = new Hangfire.RecurringJobManager();
			manager.AddOrUpdate(config.RecurringJobId, Hangfire.Common.Job.FromExpression(() => jobInstance.ExecuteService()), config.CronExpression);
		}

		private static string GetCronExpression(SchedulerConfiguration config)
		{
			string cronExpression = "";

			switch (config.IntervalTypeId)
			{
				case "CRON_EXPRESSION":
					cronExpression = config.CronExpression;
					break;
				// daily
				case "DAILY":
					cronExpression = Hangfire.Cron.Daily();
					break;
				case "DAILY_HOUR":
					cronExpression = Hangfire.Cron.Daily(config.IntervalValue.Value);
					break;
				case "DAILY_HOUR_MINUTE":
					cronExpression = Hangfire.Cron.Daily(config.IntervalValue.Value, config.IntervalValue2.Value);
					break;

				// hourly
				case "HOUR":
					cronExpression = Hangfire.Cron.Hourly();
					break;
				//case "HOUR_INTERVAL":
				//	break;
				case "HOUR_MINUTE":
					cronExpression = Hangfire.Cron.Hourly(config.IntervalValue.Value);
					break;

				// minutely
				case "MINUTE":
					cronExpression = Hangfire.Cron.Minutely();
					break;
				//case "MINUTE_INTERVAL":
				//	break;

				// monthly
				case "MONTHLY":
					cronExpression = Hangfire.Cron.Monthly();
					break;
				case "MONTHLY_DAY":
					cronExpression = Hangfire.Cron.Monthly(config.IntervalValue.Value);
					break;
				case "MONTHLY_DAY_HOUR":
					cronExpression = Hangfire.Cron.Monthly(config.IntervalValue.Value, config.IntervalValue2.Value);
					break;
				case "MONTHLY_DAY_HOUR_MINUTE":
					cronExpression = Hangfire.Cron.Monthly(config.IntervalValue.Value, config.IntervalValue2.Value, config.IntervalValue3.Value);
					break;
				//case "MONTHLY_INTERVAL":
				//	break;

				// weekly
				case "WEEKLY":
					cronExpression = Hangfire.Cron.Weekly();
					break;
				case "WEEKLY_DOW":
					cronExpression = Hangfire.Cron.Weekly((DayOfWeek)config.IntervalValue.Value);
					break;
				case "WEEKLY_DOW_HOUR":
					cronExpression = Hangfire.Cron.Weekly((DayOfWeek)config.IntervalValue.Value, config.IntervalValue2.Value);
					break;
				case "WEEKLY_DOW_HOUR_MINUTE":
					cronExpression = Hangfire.Cron.Weekly((DayOfWeek)config.IntervalValue.Value, config.IntervalValue2.Value, config.IntervalValue3.Value);
					break;

				// yearly
				case "YEARLY":
					cronExpression = Hangfire.Cron.Yearly();
					break;
				case "YEARLY_MONTH":
					cronExpression = Hangfire.Cron.Yearly(config.IntervalValue.Value);
					break;
				case "YEARLY_MONTH_DAY":
					cronExpression = Hangfire.Cron.Yearly(config.IntervalValue.Value, config.IntervalValue2.Value);
					break;
				case "YEARLY_MONTH_DAY_HOUR":
					cronExpression = Hangfire.Cron.Yearly(config.IntervalValue.Value, config.IntervalValue2.Value, config.IntervalValue3.Value);
					break;
					//case "YEARLY_MONTH_DAY_HOUR_MINUTE":
					//	cronExpression = Hangfire.Cron.Yearly(config.IntervalValue, config.IntervalValue2, config.IntervalValue3, );
					//	break;
			}

			return cronExpression;
		}

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			entity.IntervalTypeId = "NEVER";
			await RegisterJobToHangfire(entity);

			_unitOfWork.SchedulerConfigurationRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<SchedulerConfiguration> FirstAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<SchedulerConfiguration> FirstOrDefaultAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<SchedulerConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<SchedulerConfiguration> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new SchedulerConfigurationFilterSpecification(id, true);
			var schedulerConfiguration = await _unitOfWork.SchedulerConfigurationRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (schedulerConfiguration == null || includeChilds == false)
				return schedulerConfiguration;



			return schedulerConfiguration;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<SchedulerConfiguration>> ListAllAsync(List<SortingInformation<SchedulerConfiguration>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<SchedulerConfiguration>> ListAsync(
			ISpecification<SchedulerConfiguration> spec, 
			List<SortingInformation<SchedulerConfiguration>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var schedulerConfigurations = await _unitOfWork.SchedulerConfigurationRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && schedulerConfigurations?.Count > 0)
			{
				var results = new List<SchedulerConfiguration>(schedulerConfigurations);
				var schedulerConfigurationIds = schedulerConfigurations.Select(e => e.Id).ToList();



				return results;
			}

			return schedulerConfigurations;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			entity.CronExpression = GetCronExpression(entity);

			await _unitOfWork.SchedulerConfigurationRepository.ReplaceAsync(entity, entity.Id, cancellationToken);

			// update & commit
			//await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			await RegisterJobToHangfire(entity);
			
			return true;
		}


		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(SchedulerConfiguration entity, int? rowNum = null)
		{
			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");

			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(SchedulerConfiguration entity, int? rowNum = null)
		{
			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(SchedulerConfiguration entity, int? rowNum = null)
		{
			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("scheduler_configuration.html");
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

			var items = await this.ListAsync(new SchedulerConfigurationFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("scheduler_configuration.html");
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
			var downloadProcess = new DownloadProcess("scheduler_configuration") { StartTime = DateTime.Now };
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
		private string MapTemplateValue(string htmlContent, SchedulerConfiguration entity)
		{
			Dictionary<string, object> mapper = new()
				{
					{"Id",""},
					{"IntervalValue",""},
					{"IntervalValue2",""},
					{"IntervalValue3",""},
					{"CronExpression",""},
					{"RecurringJobId",""},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["IntervalType"] = entity.IntervalType.Name;
				mapper["IntervalValue"] = entity.IntervalValue?.ToString("#,#0");
				mapper["IntervalValue2"] = entity.IntervalValue2?.ToString("#,#0");
				mapper["IntervalValue3"] = entity.IntervalValue3?.ToString("#,#0");
				mapper["CronExpression"] = entity.CronExpression;
				mapper["JobType"] = entity.JobType.JobName;
				mapper["RecurringJobId"] = entity.RecurringJobId;

			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			int? id = null, List<string> intervalTypes = null, List<int> intervalValues = null, List<int> intervalValue2s = null, List<int> intervalValue3s = null, List<string> cronExpressions = null, List<int> jobTypes = null, List<string> recurringJobIds = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("scheduler_configuration") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, intervalTypes, intervalValues, intervalValue2s, intervalValue3s, cronExpressions, jobTypes, recurringJobIds,
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
			int? id = null, List<string> intervalTypes = null, List<int> intervalValues = null, List<int> intervalValue2s = null, List<int> intervalValue3s = null, List<string> cronExpressions = null, List<int> jobTypes = null, List<string> recurringJobIds = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				SchedulerConfigurationFilterSpecification filterSpec = null;
				if (id.HasValue)
					filterSpec = new SchedulerConfigurationFilterSpecification(id.Value);
				else
					filterSpec = new SchedulerConfigurationFilterSpecification(exact)
					{

						Id = id, 
						IntervalTypeIds = intervalTypes, 
						IntervalValues = intervalValues, 
						IntervalValue2s = intervalValue2s, 
						IntervalValue3s = intervalValue3s, 
						CronExpressions = cronExpressions, 
						JobTypeIds = jobTypes, 
						RecurringJobIds = recurringJobIds
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<SchedulerConfiguration>(excelFilename, "schedulerConfiguration.json", results) == false)
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
		public async Task<List<SchedulerConfiguration>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<SchedulerConfiguration>(tempExcelFile, "schedulerConfiguration.json");
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
					await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.SchedulerConfigurationRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync(cancellationToken);

			return result;
		}

		private async Task RunMasterDataValidation(List<SchedulerConfiguration> result, CancellationToken cancellationToken)
		{
			var excelSchedulerCronIntervalIds1 = result.Where(s => !string.IsNullOrEmpty(s.IntervalTypeId)).Select(s => s.IntervalTypeId).ToList();
			var SchedulerCronIntervals = await _unitOfWork.SchedulerCronIntervalRepository.ListAsync(new SchedulerCronIntervalFilterSpecification(excelSchedulerCronIntervalIds1), null, cancellationToken);
			var SchedulerCronIntervalIds = SchedulerCronIntervals.Select(e => e.Id);
			var excelJobConfigurationIds1 = result.Where(s => s.JobTypeId.HasValue).Select(s => s.JobTypeId.Value).ToList();
			var JobConfigurations = await _unitOfWork.JobConfigurationRepository.ListAsync(new JobConfigurationFilterSpecification(excelJobConfigurationIds1), null, cancellationToken);
			var JobConfigurationIds = JobConfigurations.Select(e => e.Id);

		}

		private void SetUploadDraftFlags(List<SchedulerConfiguration> result)
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
			var spec = new SchedulerConfigurationFilterSpecification()
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
		public async Task<bool> ProcessUploadedFile(IEnumerable<SchedulerConfiguration> schedulerConfigurations, CancellationToken cancellationToken = default)
		{
			if (schedulerConfigurations == null)
				throw new ArgumentNullException(nameof(schedulerConfigurations));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in schedulerConfigurations)
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
			var spec = new SchedulerConfigurationFilterSpecification()
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
				var ws = package.Workbook.Worksheets.Add("SchedulerConfiguration");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "Interval Type";
				ws.Cells[1, 4].Value = "Interval Value";
				ws.Cells[1, 5].Value = "Interval Value 2";
				ws.Cells[1, 6].Value = "Interval Value 3";
				ws.Cells[1, 7].Value = "Cron Expression";
				ws.Cells[1, 8].Value = "Job Type";
				ws.Cells[1, 9].Value = "Recurring Job Id";
				ws.Cells[1, 10].Value = "Status";
				ws.Cells[1, 11].Value = "Message";



				int row = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.IntervalValue;
					ws.Cells[row, 4].Value = item.IntervalValue2;
					ws.Cells[row, 5].Value = item.IntervalValue3;
					ws.Cells[row, 6].Value = item.CronExpression;
					ws.Cells[row, 7].Value = item.RecurringJobId;
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
		public async Task<SchedulerConfiguration> CreateDraft(CancellationToken cancellation)
		{
			var spec = new SchedulerConfigurationFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordIdIsNull = true,
				RecordEditedBy = _userName
			}.BuildSpecification();
			var count = await _unitOfWork.SchedulerConfigurationRepository.CountAsync(spec, cancellation);
			if (count > 0)
				return await _unitOfWork.SchedulerConfigurationRepository.FirstOrDefaultAsync(spec, cancellation);

            var entity = new SchedulerConfiguration
            {
                IsDraftRecord = 1,
                MainRecordId = null,
                RecordEditedBy = _userName,
                RecordActionDate = DateTime.Now
            };

            AssignCreatorAndCompany(entity);

			await _unitOfWork.SchedulerConfigurationRepository.AddAsync(entity, cancellation);
			await _unitOfWork.CommitAsync(cancellation);
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<SchedulerConfiguration> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new SchedulerConfigurationFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Scheduler Configuration dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new SchedulerConfigurationFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.SchedulerConfigurationRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.SchedulerConfigurationRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Scheduler Configuration");
				return null;
			}

			return await _unitOfWork.SchedulerConfigurationRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(SchedulerConfiguration schedulerConfiguration, CancellationToken cancellationToken)
		{
			var id = schedulerConfiguration.Id;
			var originalValue = await _unitOfWork.SchedulerConfigurationRepository.FirstOrDefaultAsync(new SchedulerConfigurationFilterSpecification(id), cancellationToken);

			if (originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(schedulerConfiguration.IntervalTypeId)) originalValue.IntervalTypeId = schedulerConfiguration.IntervalTypeId;
			originalValue.IntervalValue = schedulerConfiguration.IntervalValue.Value;
			originalValue.IntervalValue2 = schedulerConfiguration.IntervalValue2.Value;
			originalValue.IntervalValue3 = schedulerConfiguration.IntervalValue3.Value;
			originalValue.CronExpression = GetCronExpression(schedulerConfiguration);
			originalValue.JobTypeId = schedulerConfiguration.JobTypeId.Value;
			originalValue.RecurringJobId = schedulerConfiguration.RecurringJobId;


			// pastikan data belongsTo & hasMany tidak ikut
			schedulerConfiguration.IntervalType = null;
			schedulerConfiguration.JobType = null;


			AssignUpdater(originalValue);
			await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(originalValue, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			await RegisterJobToHangfire(originalValue);

			return true;
		}
		#endregion

		#region appgen: commit draft
		public async Task<int> CommitDraft(int id, CancellationToken cancellationToken)
		{
			int resultId = 0;
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return 0;

			SchedulerConfiguration destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.IntervalType = recoveryRecord.IntervalType;
				destinationRecord.IntervalType = null;
				destinationRecord.IntervalValue = recoveryRecord.IntervalValue;
				destinationRecord.IntervalValue2 = recoveryRecord.IntervalValue2;
				destinationRecord.IntervalValue3 = recoveryRecord.IntervalValue3;
				destinationRecord.CronExpression = recoveryRecord.CronExpression;
				destinationRecord.JobType = recoveryRecord.JobType;
				destinationRecord.JobType = null;
				destinationRecord.RecurringJobId = recoveryRecord.RecurringJobId;


				await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(destinationRecord, cancellationToken);
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
			await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(recoveryRecord, cancellationToken);
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
			await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new SchedulerConfigurationFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new();
			var datas = await _unitOfWork.SchedulerConfigurationRepository.ListAsync(spec, null, cancellationToken);
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
			var spec = new SchedulerConfigurationFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.SchedulerConfigurationRepository.ListAsync(spec, null, cancellationToken);
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
