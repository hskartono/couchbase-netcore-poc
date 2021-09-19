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

namespace AppCoreApi.PublicApi.Features.SchedulerCronIntervals
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class SchedulerCronIntervalController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "scheduler_cron_interval";
		private readonly ILogger<SchedulerCronIntervalController> _logger;
		private readonly ISchedulerCronIntervalService _schedulerCronIntervalService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public SchedulerCronIntervalController(
			ISchedulerCronIntervalService schedulerCronIntervalService,
			IMapper mapper,
			ILogger<SchedulerCronIntervalController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_schedulerCronIntervalService = schedulerCronIntervalService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<SchedulerCronIntervalDTO>>> GetListAsync(
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
			var totalItems = await _schedulerCronIntervalService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _schedulerCronIntervalService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<SchedulerCronIntervalDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<SchedulerCronIntervalDTO>));
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
			var items = await _schedulerCronIntervalService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<SchedulerCronIntervalDTO>> GetIdAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _schedulerCronIntervalService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), id);
			return _mapper.Map<SchedulerCronIntervalDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] SchedulerCronIntervalDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<SchedulerCronInterval>(schedulerCronInterval);

            // untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
            CleanReferenceObject(newItem);

			newItem = await _schedulerCronIntervalService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] SchedulerCronIntervalDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new SchedulerCronIntervalFilterSpecification(schedulerCronInterval.Id, true);
			var rowCount = await _schedulerCronIntervalService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), schedulerCronInterval.Id);

			// bind to old item


			var objItem = _mapper.Map<SchedulerCronInterval>(schedulerCronInterval);

            // untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
            CleanReferenceObject(objItem);

			var result = await _schedulerCronIntervalService.UpdateAsync(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = objItem.Id }, null);
		}
		#endregion

		#region appgen: delete record
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowDelete) return ValidationProblem();
			// validate if data exists
			var itemToDelete = await _schedulerCronIntervalService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), id);

			// delete data
			var result = await _schedulerCronIntervalService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
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
			var newItem = await _schedulerCronIntervalService.CreateDraft(cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<SchedulerCronIntervalDTO>(newItem));
		}
		#endregion

		#region appgen: edit draft
		
		[HttpGet]
		[Route("edit/{id}")]
		public async Task<ActionResult> CreateEditDraftAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var editItem = await _schedulerCronIntervalService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<SchedulerCronIntervalDTO>(editItem));
		}
		
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(string id, [FromBody] SchedulerCronIntervalDTO schedulerCronIntervalDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new SchedulerCronIntervalFilterSpecification(schedulerCronIntervalDto.Id, true);
			var rowCount = await _schedulerCronIntervalService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), schedulerCronIntervalDto.Id);

			var schedulerCronInterval = _mapper.Map<SchedulerCronInterval>(schedulerCronIntervalDto);
			var result = await _schedulerCronIntervalService.PatchDraft(schedulerCronInterval, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id }, null);
		}
		#endregion

		#region appgen: commit changes
		
		[HttpPut]
		[Route("commit/{id}")]
		public async Task<ActionResult> CommitDraftAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var mainRecordId = await _schedulerCronIntervalService.CommitDraft(id, cancellationToken);
			if (string.IsNullOrEmpty(mainRecordId))
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id }, null);
		}
		
		#endregion

		#region appgen: discard changes
		[HttpDelete]
		[Route("discard/{id}")]
		public async Task<IActionResult> DiscardAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _schedulerCronIntervalService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
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
			var draftList = await _schedulerCronIntervalService.GetDraftList(cancellationToken);
			return Ok(draftList);
		}
		
		#endregion

		#region appgen: get current editor
		
		[HttpGet]
		[Route("currenteditor/{id}")]
		public async Task<IActionResult> CurrentEditorAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var currentEditors = await _schedulerCronIntervalService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
		}
		
		#endregion



		#endregion

		#region PDF operation

		#region appgen: generate single pdf
		[HttpPost]
		[Route("singlepagepdf")]
		public async Task<IActionResult> SinglePdfAsync(Dictionary<string, List<string>> ids, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowPrint) return ValidationProblem();
			var id = ids["ids"][0];
			var item = await _schedulerCronIntervalService.GetByIdAsync(id, cancellationToken);
			var result = _schedulerCronIntervalService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
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
		public async Task<IActionResult> MultipagePdfAsync([FromBody] Dictionary<string, List<string>> ids, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowPrint) return ValidationProblem();
			var idToPrint = ids["ids"];
			string key = await _schedulerCronIntervalService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
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
		public async Task<ActionResult<IEnumerable<SchedulerCronIntervalDTO>>> UploadAsync(
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

			var result = await _schedulerCronIntervalService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}
			
			List<SchedulerCronIntervalDTO> resultDto = result.Select(_mapper.Map<SchedulerCronIntervalDTO>).ToList();
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
			var result = await _schedulerCronIntervalService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
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
			var result = await _schedulerCronIntervalService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "SchedulerCronInterval.xlsx";
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
			string id = (filterParams.ContainsKey("id") ? filterParams["id"][0] : string.Empty);
			#endregion

			#region Name
			List<string> name = (filterParams.ContainsKey("name") ? filterParams["name"] : null);
			#endregion



			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);

			string key = await _schedulerCronIntervalService.GenerateExcelBackgroundProcess(excelFile, 
				id, name, 
				exact, cancellationToken);
			Dictionary<string, string> result = new() { {"id", key} };
			return Ok(result);
/*
			string generatedFilename = await _schedulerCronIntervalService.GenerateExcel(excelFile, null,
				id, name, 
				exact, cancellationToken);
			fileName = "SchedulerCronInterval.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);
*/
		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private static SchedulerCronIntervalFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<SchedulerCronInterval>> sorting = null)
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
			string id = (filterParams.ContainsKey("id") ? filterParams["id"][0] : string.Empty);
			#endregion

			#region Name
			List<string> name = (filterParams.ContainsKey("name") ? filterParams["name"] : null);
			#endregion


			
			// RECOVERY FILTER	
			string mainRecordId = (filterParams.ContainsKey("mainRecordId") ? filterParams["mainRecordId"][0] : null);
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
				return new SchedulerCronIntervalFilterSpecification(exact)
				{

					Id = id, 
					Names = name,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new SchedulerCronIntervalFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					Names = name,

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
		private static List<SortingInformation<SchedulerCronInterval>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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
		
		private static List<SortingInformation<SchedulerCronInterval>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<SchedulerCronInterval>> sortingSpec = new();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.Id, sortingOrder));
						break;
					case "name":
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.Name, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.Id, sortingOrder));
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
		private static void CleanReferenceObject(SchedulerCronInterval entity)
		{
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_schedulerCronIntervalService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
