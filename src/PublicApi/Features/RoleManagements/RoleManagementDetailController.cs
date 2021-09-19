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

namespace AppCoreApi.PublicApi.Features.RoleManagements
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class RoleManagementDetailController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "role_management_detail";
		private readonly ILogger<RoleManagementDetailController> _logger;
		private readonly IRoleManagementDetailService _roleManagementDetailService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public RoleManagementDetailController(
			IRoleManagementDetailService roleManagementDetailService,
			IMapper mapper,
			ILogger<RoleManagementDetailController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_roleManagementDetailService = roleManagementDetailService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<RoleManagementDetailDTO>>> GetListAsync(
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
			var totalItems = await _roleManagementDetailService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _roleManagementDetailService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<RoleManagementDetailDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<RoleManagementDetailDTO>));
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
			var items = await _roleManagementDetailService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<RoleManagementDetailDTO>> GetIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _roleManagementDetailService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(RoleManagementDetail), id);
			return _mapper.Map<RoleManagementDetailDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] RoleManagementDetailDTO roleManagementDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<RoleManagementDetail>(roleManagementDetail);

            // untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
            CleanReferenceObject(newItem);

			newItem = await _roleManagementDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] RoleManagementDetailDTO roleManagementDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new RoleManagementDetailFilterSpecification(int.Parse(roleManagementDetail.Id), true);
			var rowCount = await _roleManagementDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(RoleManagementDetail), roleManagementDetail.Id);

			// bind to old item


			var objItem = _mapper.Map<RoleManagementDetail>(roleManagementDetail);

            // untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
            CleanReferenceObject(objItem);

			var result = await _roleManagementDetailService.PatchDraft(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
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
			var itemToDelete = await _roleManagementDetailService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(RoleManagementDetail), id);

			// delete data
			var result = await _roleManagementDetailService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
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
		public async Task<ActionResult> CreateDraftAsync([FromBody] RoleManagementDetailDTO roleManagementDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			var newItem = _mapper.Map<RoleManagementDetail>(roleManagementDetail);
			 newItem = await _roleManagementDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<RoleManagementDetailDTO>(newItem));
		}
		#endregion

		#region appgen: edit draft
		/*
		[HttpGet]
		[Route("edit/{id}")]
		public async Task<ActionResult> CreateEditDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var editItem = await _roleManagementDetailService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<RoleManagementDetailDTO>(editItem));
		}
		*/
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(int id, [FromBody] RoleManagementDetailDTO roleManagementDetailDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new RoleManagementDetailFilterSpecification(int.Parse(roleManagementDetailDto.Id), true);
			var rowCount = await _roleManagementDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(RoleManagementDetail), roleManagementDetailDto.Id);

			var roleManagementDetail = _mapper.Map<RoleManagementDetail>(roleManagementDetailDto);
			var result = await _roleManagementDetailService.PatchDraft(roleManagementDetail, cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id }, null);
		}
		#endregion

		#region appgen: commit changes
		/*
		[HttpPut]
		[Route("commit/{id}")]
		public async Task<ActionResult> CommitDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var mainRecordId = await _roleManagementDetailService.CommitDraft(id, cancellationToken);
			if (mainRecordId <= 0)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		}
		*/
		#endregion

		#region appgen: discard changes
		[HttpDelete]
		[Route("discard/{id}")]
		public async Task<IActionResult> DiscardAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _roleManagementDetailService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#region appgen: get draft list
		/*
		[HttpGet]
		[Route("draftlist")]
		public async Task<IActionResult> DraftListAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var draftList = await _roleManagementDetailService.GetDraftList(cancellationToken);
			return Ok(draftList);
		}
		*/
		#endregion

		#region appgen: get current editor
		/*
		[HttpGet]
		[Route("currenteditor/{id}")]
		public async Task<IActionResult> CurrentEditorAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var currentEditors = await _roleManagementDetailService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
		}
		*/
		#endregion

		#region appgen: replace async
		[HttpPost]
		[Route("replace")]
		public async Task<ActionResult> ReplaceAsync(
			[FromBody] List<RoleManagementDetail> roleManagementDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = roleManagementDetails.Select(_mapper.Map<RoleManagementDetail>).ToList();

			foreach (var item in newItems)
                CleanReferenceObject(item);

			newItems = await _roleManagementDetailService.ReplaceDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return Ok();
		}
		#endregion

		#region appgen: add async
		[HttpPost]
		[Route("add")]
		public async Task<ActionResult> AddAsync(
			[FromBody] List<RoleManagementDetail> roleManagementDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = roleManagementDetails.Select(_mapper.Map<RoleManagementDetail>).ToList();

			foreach(var item in newItems)
                CleanReferenceObject(item);

			newItems = await _roleManagementDetailService.AddDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItems[0].Id }, null);
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
			var item = await _roleManagementDetailService.GetByIdAsync(id, cancellationToken);
			var result = _roleManagementDetailService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
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
			string key = await _roleManagementDetailService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
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
		public async Task<ActionResult<IEnumerable<RoleManagementDetailDTO>>> UploadAsync(
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

			var result = await _roleManagementDetailService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
				return ValidationProblem();
			}
			
			List<RoleManagementDetailDTO> resultDto = result.Select(_mapper.Map<RoleManagementDetailDTO>).ToList();
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
			var result = await _roleManagementDetailService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleManagementDetailService.Errors);
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
			var result = await _roleManagementDetailService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "RoleManagementDetail.xlsx";
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

			#region Role Management
			List<int> roleManagementId = null;
			if (filterParams.ContainsKey("roleManagementId")) 
			{
				roleManagementId = new List<int>();
				foreach(var item in filterParams["roleManagementId"])
				{
					var data = int.Parse(item);
					roleManagementId.Add(data);
				}
			}
			#endregion

			#region Function Info
			List<string> functionInfoId = (filterParams.ContainsKey("functionInfoId") ? filterParams["functionInfoId"] : null);
			#endregion

			#region Allow Create
			List<bool> allowCreate = null;
			if (filterParams.ContainsKey("allowCreate")) 
			{
				allowCreate = new List<bool>();
				foreach(var item in filterParams["allowCreate"])
				{
					var data = (item == "1");
					allowCreate.Add(data);
				}
			}
			#endregion

			#region Allow Read
			List<bool> allowRead = null;
			if (filterParams.ContainsKey("allowRead")) 
			{
				allowRead = new List<bool>();
				foreach(var item in filterParams["allowRead"])
				{
					var data = (item == "1");
					allowRead.Add(data);
				}
			}
			#endregion

			#region Allow Update
			List<bool> allowUpdate = null;
			if (filterParams.ContainsKey("allowUpdate")) 
			{
				allowUpdate = new List<bool>();
				foreach(var item in filterParams["allowUpdate"])
				{
					var data = (item == "1");
					allowUpdate.Add(data);
				}
			}
			#endregion

			#region Allow Delete
			List<bool> allowDelete = null;
			if (filterParams.ContainsKey("allowDelete")) 
			{
				allowDelete = new List<bool>();
				foreach(var item in filterParams["allowDelete"])
				{
					var data = (item == "1");
					allowDelete.Add(data);
				}
			}
			#endregion

			#region Show In Menu
			List<bool> showInMenu = null;
			if (filterParams.ContainsKey("showInMenu")) 
			{
				showInMenu = new List<bool>();
				foreach(var item in filterParams["showInMenu"])
				{
					var data = (item == "1");
					showInMenu.Add(data);
				}
			}
			#endregion

			#region Allow Download
			List<bool> allowDownload = null;
			if (filterParams.ContainsKey("allowDownload")) 
			{
				allowDownload = new List<bool>();
				foreach(var item in filterParams["allowDownload"])
				{
					var data = (item == "1");
					allowDownload.Add(data);
				}
			}
			#endregion

			#region Allow Print
			List<bool> allowPrint = null;
			if (filterParams.ContainsKey("allowPrint")) 
			{
				allowPrint = new List<bool>();
				foreach(var item in filterParams["allowPrint"])
				{
					var data = (item == "1");
					allowPrint.Add(data);
				}
			}
			#endregion

			#region Allow Upload
			List<bool> allowUpload = null;
			if (filterParams.ContainsKey("allowUpload")) 
			{
				allowUpload = new List<bool>();
				foreach(var item in filterParams["allowUpload"])
				{
					var data = (item == "1");
					allowUpload.Add(data);
				}
			}
			#endregion



			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);
/*
			string key = await _roleManagementDetailService.GenerateExcelBackgroundProcess(excelFile, 
				id, roleManagementId, functionInfoId, allowCreate, allowRead, allowUpdate, allowDelete, showInMenu, allowDownload, allowPrint, allowUpload, 
				exact, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>() { {"id", key} };
			return Ok(result);
*/
			string generatedFilename = await _roleManagementDetailService.GenerateExcel(excelFile, null,
				id, roleManagementId, functionInfoId, allowCreate, allowRead, allowUpdate, allowDelete, showInMenu, allowDownload, allowPrint, allowUpload, 
				exact, cancellationToken);
			fileName = "RoleManagementDetail.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);

		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private static RoleManagementDetailFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<RoleManagementDetail>> sorting = null)
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

			#region Role Management
			List<int> roleManagementId = null;
			if (filterParams.ContainsKey("roleManagementId")) 
			{
				roleManagementId = new List<int>();
				foreach(var item in filterParams["roleManagementId"])
				{
					var data = int.Parse(item);
					roleManagementId.Add(data);
				}
			}
			#endregion

			#region Function Info
			List<string> functionInfoId = (filterParams.ContainsKey("functionInfoId") ? filterParams["functionInfoId"] : null);
			#endregion

			#region Allow Create
			List<bool> allowCreate = null;
			if (filterParams.ContainsKey("allowCreate")) 
			{
				allowCreate = new List<bool>();
				foreach(var item in filterParams["allowCreate"])
				{
					var data = (item == "1");
					allowCreate.Add(data);
				}
			}
			#endregion

			#region Allow Read
			List<bool> allowRead = null;
			if (filterParams.ContainsKey("allowRead")) 
			{
				allowRead = new List<bool>();
				foreach(var item in filterParams["allowRead"])
				{
					var data = (item == "1");
					allowRead.Add(data);
				}
			}
			#endregion

			#region Allow Update
			List<bool> allowUpdate = null;
			if (filterParams.ContainsKey("allowUpdate")) 
			{
				allowUpdate = new List<bool>();
				foreach(var item in filterParams["allowUpdate"])
				{
					var data = (item == "1");
					allowUpdate.Add(data);
				}
			}
			#endregion

			#region Allow Delete
			List<bool> allowDelete = null;
			if (filterParams.ContainsKey("allowDelete")) 
			{
				allowDelete = new List<bool>();
				foreach(var item in filterParams["allowDelete"])
				{
					var data = (item == "1");
					allowDelete.Add(data);
				}
			}
			#endregion

			#region Show In Menu
			List<bool> showInMenu = null;
			if (filterParams.ContainsKey("showInMenu")) 
			{
				showInMenu = new List<bool>();
				foreach(var item in filterParams["showInMenu"])
				{
					var data = (item == "1");
					showInMenu.Add(data);
				}
			}
			#endregion

			#region Allow Download
			List<bool> allowDownload = null;
			if (filterParams.ContainsKey("allowDownload")) 
			{
				allowDownload = new List<bool>();
				foreach(var item in filterParams["allowDownload"])
				{
					var data = (item == "1");
					allowDownload.Add(data);
				}
			}
			#endregion

			#region Allow Print
			List<bool> allowPrint = null;
			if (filterParams.ContainsKey("allowPrint")) 
			{
				allowPrint = new List<bool>();
				foreach(var item in filterParams["allowPrint"])
				{
					var data = (item == "1");
					allowPrint.Add(data);
				}
			}
			#endregion

			#region Allow Upload
			List<bool> allowUpload = null;
			if (filterParams.ContainsKey("allowUpload")) 
			{
				allowUpload = new List<bool>();
				foreach(var item in filterParams["allowUpload"])
				{
					var data = (item == "1");
					allowUpload.Add(data);
				}
			}
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
			bool? draftFromUpload = filterParams.ContainsKey("draftFromUpload") ? bool.Parse(filterParams["draftFromUpload"][0]) : null;
			if (filterParams.ContainsKey("draftMode"))
			{
				if (filterParams["draftMode"][0] == "true") filterParams["draftMode"][0] = "1";
				if (filterParams["draftMode"][0] == "false") filterParams["draftMode"][0] = "0";
			}
			int draftMode = (filterParams.ContainsKey("draftMode") ? int.Parse(filterParams["draftMode"][0]) : -1);

			if (pageSize == 0)
			{
				return new RoleManagementDetailFilterSpecification(exact)
				{

					Id = id, 
					RoleManagementIds = roleManagementId, 
					FunctionInfoIds = functionInfoId, 
					AllowCreates = allowCreate, 
					AllowReads = allowRead, 
					AllowUpdates = allowUpdate, 
					AllowDeletes = allowDelete, 
					ShowInMenus = showInMenu, 
					AllowDownloads = allowDownload, 
					AllowPrints = allowPrint, 
					AllowUploads = allowUpload,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new RoleManagementDetailFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					RoleManagementIds = roleManagementId, 
					FunctionInfoIds = functionInfoId, 
					AllowCreates = allowCreate, 
					AllowReads = allowRead, 
					AllowUpdates = allowUpdate, 
					AllowDeletes = allowDelete, 
					ShowInMenus = showInMenu, 
					AllowDownloads = allowDownload, 
					AllowPrints = allowPrint, 
					AllowUploads = allowUpload,

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
		private static List<SortingInformation<RoleManagementDetail>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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
		
		private static List<SortingInformation<RoleManagementDetail>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<RoleManagementDetail>> sortingSpec = new();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.Id, sortingOrder));
						break;
					case "rolemanagement":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.RoleManagement.Name, sortingOrder));
						break;
					case "functioninfo":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.FunctionInfo.Name, sortingOrder));
						break;
					case "allowcreate":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowCreate, sortingOrder));
						break;
					case "allowread":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowRead, sortingOrder));
						break;
					case "allowupdate":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowUpdate, sortingOrder));
						break;
					case "allowdelete":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowDelete, sortingOrder));
						break;
					case "showinmenu":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.ShowInMenu, sortingOrder));
						break;
					case "allowdownload":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowDownload, sortingOrder));
						break;
					case "allowprint":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowPrint, sortingOrder));
						break;
					case "allowupload":
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.AllowUpload, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<RoleManagementDetail>(p => p.Id, sortingOrder));
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
		private static void CleanReferenceObject(RoleManagementDetail entity)
		{
			entity.RoleManagement = null;
			entity.FunctionInfo = null;


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_roleManagementDetailService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
