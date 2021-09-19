using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Exceptions;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.DownloadProcesses
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class DownloadProcessController : BaseAPIController
	{
		private const string functionId = "download_process";
		private readonly ILogger<DownloadProcessController> _logger;
		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		public DownloadProcessController(
			IDownloadProcessService downloadProcessService,
			IMapper mapper,
			ILogger<DownloadProcessController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_downloadProcessService = downloadProcessService;
			_downloadProcessService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<DownloadProcessDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter, exact);
			var totalItems = await _downloadProcessService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _downloadProcessService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<DownloadProcessDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<DownloadProcessDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<DownloadProcessDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			return _mapper.Map<DownloadProcessDTO>(item);
		}

		// GET api/v1/[controller]/status/1
		[HttpGet]
		[Route("status/{id}")]
		public async Task<ActionResult<Dictionary<string, string>>> GetStatusAsync(string id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByJobIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			var downloadUrl = _uriComposer.ComposeDownloadUri(item.FileName);
			return new Dictionary<string, string>()
			{
				{"status", item.Status},
				{"filename", downloadUrl }
			};
		}

		[HttpGet]
		[Route("download/{id}")]
		public async Task<IActionResult> DownloadAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			var downloadUrl = _uriComposer.ComposeDownloadUri(item.FileName);
			var result = new Dictionary<string, string>()
			{
				{"download", downloadUrl}
			};

			return Ok(result);
		}

		[HttpGet]
		[Route("directdownload/{id}")]
		public async Task<IActionResult> DirectDownloadAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			var filePath = _uriComposer.ComposeDownloadPath(item.FileName);
			var fileExtension = Path.GetExtension(filePath).ToLower();

			var mimeType = $"application/{fileExtension}";
			if (fileExtension == "pdf")
			{
				mimeType = "application/pdf";
			}
			else if (fileExtension == "xlsx")
			{
				mimeType = "application/xlsx";
			}

			var fileName = $"download{fileExtension}";
			byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
			return File(fileBytes, mimeType, fileName);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] DownloadProcessDTO downloadProcessDto, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<DownloadProcess>(downloadProcessDto);
			newItem = await _downloadProcessService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_downloadProcessService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] DownloadProcessDTO downloadProcessDto, CancellationToken cancellationToken)
		{
			var specFilter = new DownloadProcessFilterSpecification(downloadProcessDto.Id);
			var rowCount = await _downloadProcessService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(downloadProcessDto), downloadProcessDto.Id);

			var item = _mapper.Map<DownloadProcess>(downloadProcessDto);

			var result = await _downloadProcessService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_downloadProcessService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
		}

		// DELETE api/v1/[controller]/id
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteItemAsync(int id, CancellationToken cancellationToken)
		{
			// validate if data exists
			var itemToDelete = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			// delete data
			var result = await _downloadProcessService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_downloadProcessService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		#region appgen: generate filter
		private static DownloadProcessFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters,
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<DownloadProcess>> sorting = null)
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

			#region Job Id
			List<string> jobId = (filterParams.ContainsKey("jobId") ? filterParams["jobId"] : null);
			#endregion

			#region Function Id
			List<string> functionId = (filterParams.ContainsKey("functionId") ? filterParams["functionId"] : null);
			#endregion

			#region File Name
			List<string> fileName = (filterParams.ContainsKey("fileName") ? filterParams["fileName"] : null);
			#endregion

			#region Start Time
			DateTime? startTimeFrom = (filterParams.ContainsKey("startTimeFrom") ? DateTime.Parse(filterParams["startTimeFrom"][0]) : null);
			DateTime? startTimeTo = (filterParams.ContainsKey("startTimeTo") ? DateTime.Parse(filterParams["startTimeTo"][0]) : null);
			#endregion

			#region End Time
			DateTime? endTimeFrom = (filterParams.ContainsKey("endTimeFrom") ? DateTime.Parse(filterParams["endTimeFrom"][0]) : null);
			DateTime? endTimeTo = (filterParams.ContainsKey("endTimeTo") ? DateTime.Parse(filterParams["endTimeTo"][0]) : null);
			#endregion

			#region Status
			List<string> status = (filterParams.ContainsKey("status") ? filterParams["status"] : null);
			#endregion

			#region Error Message
			List<string> errorMessage = (filterParams.ContainsKey("errorMessage") ? filterParams["errorMessage"] : null);
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
				return new DownloadProcessFilterSpecification(exact)
				{

					Id = id,
					JobIds = jobId,
					FunctionIds = functionId,
					FileNames = fileName,
					StartTimeFrom = startTimeFrom,
					StartTimeTo = startTimeTo,
					EndTimeFrom = endTimeFrom,
					EndTimeTo = endTimeTo,
					Statuss = status,
					ErrorMessages = errorMessage,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus)draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new DownloadProcessFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

				Id = id,
				JobIds = jobId,
				FunctionIds = functionId,
				FileNames = fileName,
				StartTimeFrom = startTimeFrom,
				StartTimeTo = startTimeTo,
				EndTimeFrom = endTimeFrom,
				EndTimeTo = endTimeTo,
				Statuss = status,
				ErrorMessages = errorMessage,

				MainRecordId = mainRecordId,
				MainRecordIdIsNull = mainRecordIsNull,
				RecordEditedBy = recordEditedBy,
				DraftFromUpload = draftFromUpload,
				ShowDraftList = (BaseEntity.DraftStatus)draftMode
			}
			.BuildSpecification(true, sorting);
		}
		#endregion

		#region appgen: generate sorting
		private static List<SortingInformation<DownloadProcess>> GenerateSortingSpec(Dictionary<string, bool> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;
			var newDict = new Dictionary<string, int>();
			foreach (var key in sorting.Keys)
			{
				if (sorting[key] == true)
					newDict.Add(key, 1);
				else
					newDict.Add(key, 0);
			}

			return GenerateSortingSpec(newDict);
		}

		private static List<SortingInformation<DownloadProcess>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<DownloadProcess>> sortingSpec = new ();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.Id, sortingOrder));
						break;
					case "jobid":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.JobId, sortingOrder));
						break;
					case "functionid":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.FunctionId, sortingOrder));
						break;
					case "filename":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.FileName, sortingOrder));
						break;
					case "starttime":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.StartTime, sortingOrder));
						break;
					case "endtime":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.EndTime, sortingOrder));
						break;
					case "status":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.Status, sortingOrder));
						break;
					case "errormessage":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.ErrorMessage, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.Id, sortingOrder));
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
	}
}
