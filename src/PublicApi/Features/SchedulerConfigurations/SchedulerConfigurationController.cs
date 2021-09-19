using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Exceptions;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.SchedulerConfigurations
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class SchedulerConfigurationController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "scheduler_configuration";
		private readonly ILogger<SchedulerConfigurationController> _logger;
		private readonly ISchedulerConfigurationService _schedulerConfigurationService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public SchedulerConfigurationController(
			ISchedulerConfigurationService schedulerConfigurationService,
			IMapper mapper,
			ILogger<SchedulerConfigurationController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_schedulerConfigurationService = schedulerConfigurationService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<SchedulerConfigurationDTO>>> GetListAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellation = default)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
            CleanFilter(filter);
			var filterSpec = GenerateFilter(filter, exact);
			var totalItems = await _schedulerConfigurationService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _schedulerConfigurationService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<SchedulerConfigurationDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<SchedulerConfigurationDTO>));
			return Ok(model);
		}
		#endregion

		#region appgen: get id list
		[HttpGet]
		[Route("ids")]
		public async Task<ActionResult<List<int>>> GetIdsAsync(
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, bool> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellation = default)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
            CleanFilter(filter);
			var sortingSpec = GenerateSortingSpec(sorting);
			var filterSpec = GenerateFilter(filter, exact, 0, 0, sortingSpec);
			var items = await _schedulerConfigurationService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<SchedulerConfigurationDTO>> GetIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _schedulerConfigurationService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), id);
			return _mapper.Map<SchedulerConfigurationDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] SchedulerConfigurationDTO schedulerConfiguration, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<SchedulerConfiguration>(schedulerConfiguration);

            // untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
            CleanReferenceObject(newItem);

			newItem = await _schedulerConfigurationService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] SchedulerConfigurationDTO schedulerConfiguration, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new SchedulerConfigurationFilterSpecification(int.Parse(schedulerConfiguration.Id), true);
			var rowCount = await _schedulerConfigurationService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), schedulerConfiguration.Id);

			// bind to old item


			var objItem = _mapper.Map<SchedulerConfiguration>(schedulerConfiguration);

            // untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
            CleanReferenceObject(objItem);

			var result = await _schedulerConfigurationService.UpdateAsync(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = objItem.Id }, null);
		}
		#endregion

		#region appgen: delete record
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowDelete) return ValidationProblem();
			// validate if data exists
			var itemToDelete = await _schedulerConfigurationService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), id);

			// delete data
			var result = await _schedulerConfigurationService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#endregion

		#region Recovery Record Controller

		#region appgen: create draft
		[HttpPost]
		[Route("create")]
		public async Task<ActionResult> CreateDraftAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			var newItem = await _schedulerConfigurationService.CreateDraft(cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<SchedulerConfigurationDTO>(newItem));
		}
		#endregion

		#region appgen: edit draft
		
		[HttpGet]
		[Route("edit/{id}")]
		public async Task<ActionResult> CreateEditDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var editItem = await _schedulerConfigurationService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<SchedulerConfigurationDTO>(editItem));
		}
		
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(int id, [FromBody] SchedulerConfigurationDTO schedulerConfigurationDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new SchedulerConfigurationFilterSpecification(int.Parse(schedulerConfigurationDto.Id), true);
			var rowCount = await _schedulerConfigurationService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), schedulerConfigurationDto.Id);

			var schedulerConfiguration = _mapper.Map<SchedulerConfiguration>(schedulerConfigurationDto);
			var result = await _schedulerConfigurationService.PatchDraft(schedulerConfiguration, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id }, null);
		}
		#endregion

		#region appgen: commit changes
		
		[HttpPut]
		[Route("commit/{id}")]
		public async Task<ActionResult> CommitDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var mainRecordId = await _schedulerConfigurationService.CommitDraft(id, cancellationToken);
			if (mainRecordId <= 0)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id }, null);
		}
		
		#endregion

		#region appgen: discard changes
		[HttpDelete]
		[Route("discard/{id}")]
		public async Task<IActionResult> DiscardAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _schedulerConfigurationService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#region appgen: get draft list
		
		[HttpGet]
		[Route("draftlist")]
		public async Task<IActionResult> DraftListAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var draftList = await _schedulerConfigurationService.GetDraftList(cancellationToken);
			return Ok(draftList);
		}
		
		#endregion

		#region appgen: get current editor
		
		[HttpGet]
		[Route("currenteditor/{id}")]
		public async Task<IActionResult> CurrentEditorAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var currentEditors = await _schedulerConfigurationService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
		}
		
		#endregion



		#endregion

		#region PDF operation

		#region appgen: generate single pdf
		[HttpPost]
		[Route("singlepagepdf")]
		public async Task<IActionResult> SinglePdfAsync(Dictionary<string, List<int>> ids, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowPrint) return ValidationProblem();
			var id = ids["ids"][0];
			var item = await _schedulerConfigurationService.GetByIdAsync(id, cancellationToken);
			var result = _schedulerConfigurationService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return BadRequest(ModelState);
			}

			var downloadUrl = _uriComposer.ComposeDownloadUri(System.IO.Path.GetFileName(result));
			var pdfResult = new Dictionary<string, string>()
			{
				{"download", downloadUrl}
			};

			return Ok(pdfResult);
		}
		#endregion

		#region appgen: generate multipage pdf
		[HttpPost]
		[Route("multipagePdf")]
		public async Task<IActionResult> MultipagePdfAsync([FromBody] Dictionary<string, List<int>> ids, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowPrint) return ValidationProblem();
			var idToPrint = ids["ids"];
			string key = await _schedulerConfigurationService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
			Dictionary<string, string> result = new()
			{
				{"id", key}
			};
			return Ok(result);
		}
		#endregion

		#endregion

		#region File Upload & Download Operation

		#region appgen: upload excel file
		[HttpPost]
		[Route("upload")]
		public async Task<ActionResult<IEnumerable<SchedulerConfigurationDTO>>> UploadAsync(
			IFormFile file, 
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpload) return ValidationProblem();
			var filePath = System.IO.Path.GetTempFileName();
			using(var stream = System.IO.File.Create(filePath))
			{
				await file.CopyToAsync(stream, cancellationToken);
			}

			var result = await _schedulerConfigurationService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}
			
			List<SchedulerConfigurationDTO> resultDto = result.Select(_mapper.Map<SchedulerConfigurationDTO>).ToList();
			foreach(var item in resultDto)
			{
				if (string.IsNullOrEmpty(item.Id) || item.Id == "0")
				{
					var newguid = Guid.NewGuid().ToString();
					item.Id = newguid;
				}
			}


return CreatedAtAction(nameof(GetIdAsync), new { id = result[0].Id }, null);
		}
		#endregion

		#region appgen: confirm uploaded file
		[HttpPost]
		[Route("confirmUpload")]
		public async Task<IActionResult> ConfirmUploadAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpload) return ValidationProblem();
			var result = await _schedulerConfigurationService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return Ok();
		}
		#endregion

		#region appgen: download log
		[HttpPost]
		[Route("downloadLog")]
		public async Task<IActionResult> DownloadLogAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _schedulerConfigurationService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "SchedulerConfiguration.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(result);
			return File(fileBytes, "application/xlsx", fileName);
		}
		#endregion

		#region appgen: download excel data
		[HttpGet]
		[Route("downloaddata")]
		public async Task<IActionResult> DownloadDataAsync(
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowDownload) return ValidationProblem();
            CleanFilter(filter);
			Dictionary<string, List<string>> filterParams = new();
			foreach (string fieldName in filter.Keys)
			{
				if (filter[fieldName].Count <= 0) continue;
				filterParams.Add(fieldName, new List<string>());
				foreach (var itemValue in filter[fieldName][""])
				{
					filterParams[fieldName].Add(itemValue);
				}
			}

			#region Id
			int? id = (filterParams.ContainsKey("id") ? int.Parse(filterParams["id"][0]) : null);
			#endregion

			#region Interval Type
			List<string> intervalTypeId = (filterParams.ContainsKey("intervalTypeId") ? filterParams["intervalTypeId"] : null);
			#endregion

			#region Interval Value
			List<int> intervalValue = null;
			if (filterParams.ContainsKey("intervalValue")) 
			{
				intervalValue = new List<int>();
				foreach(var item in filterParams["intervalValue"])
				{
					var data = int.Parse(item);
					intervalValue.Add(data);
				}
			}
			#endregion

			#region Interval Value2
			List<int> intervalValue2 = null;
			if (filterParams.ContainsKey("intervalValue2")) 
			{
				intervalValue2 = new List<int>();
				foreach(var item in filterParams["intervalValue2"])
				{
					var data = int.Parse(item);
					intervalValue2.Add(data);
				}
			}
			#endregion

			#region Interval Value3
			List<int> intervalValue3 = null;
			if (filterParams.ContainsKey("intervalValue3")) 
			{
				intervalValue3 = new List<int>();
				foreach(var item in filterParams["intervalValue3"])
				{
					var data = int.Parse(item);
					intervalValue3.Add(data);
				}
			}
			#endregion

			#region Cron Expression
			List<string> cronExpression = (filterParams.ContainsKey("cronExpression") ? filterParams["cronExpression"] : null);
			#endregion

			#region Job Type
			List<int> jobTypeId = null;
			if (filterParams.ContainsKey("jobTypeId")) 
			{
				jobTypeId = new List<int>();
				foreach(var item in filterParams["jobTypeId"])
				{
					var data = int.Parse(item);
					jobTypeId.Add(data);
				}
			}
			#endregion

			#region Recurring Job Id
			List<string> recurringJobId = (filterParams.ContainsKey("recurringJobId") ? filterParams["recurringJobId"] : null);
			#endregion



			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);

			string key = await _schedulerConfigurationService.GenerateExcelBackgroundProcess(excelFile, 
				id, intervalTypeId, intervalValue, intervalValue2, intervalValue3, cronExpression, jobTypeId, recurringJobId, 
				exact, cancellationToken);
			Dictionary<string, string> result = new() { {"id", key} };
			return Ok(result);
/*
			string generatedFilename = await _schedulerConfigurationService.GenerateExcel(excelFile, null,
				id, intervalTypeId, intervalValue, intervalValue2, intervalValue3, cronExpression, jobTypeId, recurringJobId, 
				exact, cancellationToken);
			fileName = "SchedulerConfiguration.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);
*/
		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private static SchedulerConfigurationFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<SchedulerConfiguration>> sorting = null)
		{
			Dictionary<string, List<string>> filterParams = new();
			foreach (string fieldName in filters.Keys)
			{
				if (filters[fieldName].Count <= 0) continue;
				filterParams.Add(fieldName, new List<string>());
				foreach (var itemValue in filters[fieldName][""])
				{
					filterParams[fieldName].Add(itemValue);
				}
			}
			#region Id
			int? id = (filterParams.ContainsKey("id") ? int.Parse(filterParams["id"][0]) : null);
			#endregion

			#region Interval Type
			List<string> intervalTypeId = (filterParams.ContainsKey("intervalTypeId") ? filterParams["intervalTypeId"] : null);
			#endregion

			#region Interval Value
			List<int> intervalValue = null;
			if (filterParams.ContainsKey("intervalValue")) 
			{
				intervalValue = new List<int>();
				foreach(var item in filterParams["intervalValue"])
				{
					var data = int.Parse(item);
					intervalValue.Add(data);
				}
			}
			#endregion

			#region Interval Value2
			List<int> intervalValue2 = null;
			if (filterParams.ContainsKey("intervalValue2")) 
			{
				intervalValue2 = new List<int>();
				foreach(var item in filterParams["intervalValue2"])
				{
					var data = int.Parse(item);
					intervalValue2.Add(data);
				}
			}
			#endregion

			#region Interval Value3
			List<int> intervalValue3 = null;
			if (filterParams.ContainsKey("intervalValue3")) 
			{
				intervalValue3 = new List<int>();
				foreach(var item in filterParams["intervalValue3"])
				{
					var data = int.Parse(item);
					intervalValue3.Add(data);
				}
			}
			#endregion

			#region Cron Expression
			List<string> cronExpression = (filterParams.ContainsKey("cronExpression") ? filterParams["cronExpression"] : null);
			#endregion

			#region Job Type
			List<int> jobTypeId = null;
			if (filterParams.ContainsKey("jobTypeId")) 
			{
				jobTypeId = new List<int>();
				foreach(var item in filterParams["jobTypeId"])
				{
					var data = int.Parse(item);
					jobTypeId.Add(data);
				}
			}
			#endregion

			#region Recurring Job Id
			List<string> recurringJobId = (filterParams.ContainsKey("recurringJobId") ? filterParams["recurringJobId"] : null);
			#endregion


			
			// RECOVERY FILTER	
			int? mainRecordId = (filterParams.ContainsKey("mainRecordId") ? int.Parse(filterParams["mainRecordId"][0]) : null);
			bool mainRecordIsNull = (filterParams.ContainsKey("mainRecordIsNull") && bool.Parse(filterParams["mainRecordIsNull"][0]));
			string recordEditedBy = (filterParams.ContainsKey("recordEditedBy") ? filterParams["recordEditedBy"][0] : null);
			if (filterParams.ContainsKey("draftFromUpload"))
			{
				if (filterParams["draftFromUpload"][0] == "1") 
					filterParams["draftFromUpload"][0] = "true";
				else
					filterParams["draftFromUpload"][0] = "false";
			}
			bool? draftFromUpload = (filterParams.ContainsKey("draftFromUpload") && bool.Parse(filterParams["draftFromUpload"][0]));
			if (filterParams.ContainsKey("draftMode"))
			{
				if (filterParams["draftMode"][0] == "true") filterParams["draftMode"][0] = "1";
				if (filterParams["draftMode"][0] == "false") filterParams["draftMode"][0] = "0";
			}
			int draftMode = (filterParams.ContainsKey("draftMode") ? int.Parse(filterParams["draftMode"][0]) : 0);

			if (pageSize == 0)
			{
				return new SchedulerConfigurationFilterSpecification(exact)
				{

					Id = id, 
					IntervalTypeIds = intervalTypeId, 
					IntervalValues = intervalValue, 
					IntervalValue2s = intervalValue2, 
					IntervalValue3s = intervalValue3, 
					CronExpressions = cronExpression, 
					JobTypeIds = jobTypeId, 
					RecurringJobIds = recurringJobId,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new SchedulerConfigurationFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					IntervalTypeIds = intervalTypeId, 
					IntervalValues = intervalValue, 
					IntervalValue2s = intervalValue2, 
					IntervalValue3s = intervalValue3, 
					CronExpressions = cronExpression, 
					JobTypeIds = jobTypeId, 
					RecurringJobIds = recurringJobId,

				MainRecordId = mainRecordId,
				MainRecordIdIsNull = mainRecordIsNull,
				RecordEditedBy = recordEditedBy,
				DraftFromUpload = draftFromUpload,
				ShowDraftList = (BaseEntity.DraftStatus) draftMode
			}
			.BuildSpecification(true, sorting);
		}
		#endregion

		#region appgen: generate sorting
		private static List<SortingInformation<SchedulerConfiguration>> GenerateSortingSpec(Dictionary<string, bool> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;
			var newDict = new Dictionary<string, int>();
			foreach(var key in sorting.Keys)
			{
				if (sorting[key] == true)
					newDict.Add(key, 1);
				else
					newDict.Add(key, 0);
			}

			return GenerateSortingSpec(newDict);
		}
		
		private static List<SortingInformation<SchedulerConfiguration>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<SchedulerConfiguration>> sortingSpec = new();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.Id, sortingOrder));
						break;
					case "intervaltype":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.IntervalType.Name, sortingOrder));
						break;
					case "intervalvalue":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.IntervalValue, sortingOrder));
						break;
					case "intervalvalue2":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.IntervalValue2, sortingOrder));
						break;
					case "intervalvalue3":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.IntervalValue3, sortingOrder));
						break;
					case "cronexpression":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.CronExpression, sortingOrder));
						break;
					case "jobtype":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.JobType.JobName, sortingOrder));
						break;
					case "recurringjobid":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.RecurringJobId, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.Id, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
		#endregion

		#region appgen: clean filter parameters
		private static void CleanFilter(Dictionary<string, Dictionary<string, List<string>>> filter)
		{
			if (filter.ContainsKey("exact")) filter.Remove("exact");
			if (filter.ContainsKey("sorting")) filter.Remove("sorting");
			if (filter.ContainsKey("pageSize")) filter.Remove("pageSize");
			if (filter.ContainsKey("pageIndex")) filter.Remove("pageIndex");
		}
		#endregion

		#region appgen: clean reference object
		private static void CleanReferenceObject(SchedulerConfiguration entity)
		{
			entity.IntervalType = null;
			entity.JobType = null;


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_schedulerConfigurationService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
