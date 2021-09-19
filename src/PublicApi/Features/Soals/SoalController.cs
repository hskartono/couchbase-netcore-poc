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
using AppCoreApi.ApplicationCore.Specifications.Filters;

namespace AppCoreApi.PublicApi.Features.Soals
{
	//[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class SoalController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "soal";
		private readonly ILogger<SoalController> _logger;
		private readonly ISoalService _soalService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public SoalController(
			ISoalService soalService,
			IMapper mapper,
			ILogger<SoalController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_soalService = soalService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<SoalDTO>>> GetListAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellation = default)
		{
			InitUserInfo();
			//if (!AllowRead) return ValidationProblem();
			CleanFilter(filter);
			var filterSpec = GenerateFilter(filter, exact);
			var sortingSpec = GenerateSortingSpec(sorting);
			
			var totalItems = await _soalService.CountAsync(filterSpec, cancellation);

			filterSpec.SortingSpec = sortingSpec;
			filterSpec.PageSize = pageSize;
			filterSpec.PageIndex = pageIndex;
			var items = await _soalService.ListAsync(filterSpec, cancellation);

			var model = new PaginatedItemsViewModel<SoalDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<SoalDTO>));
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
			//if (!AllowRead) return ValidationProblem();
			CleanFilter(filter);
			var filterSpec = GenerateFilter(filter, exact);
			filterSpec.SortingSpec = GenerateSortingSpec(sorting);
			var items = await _soalService.ListAsync(filterSpec, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<SoalDTO>> GetIdAsync(string id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			//if (!AllowRead) return ValidationProblem();
			var item = await _soalService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(Soal), id);
			return _mapper.Map<SoalDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] SoalDTO soal, CancellationToken cancellationToken)
		{
			InitUserInfo();
			//if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<Soal>(soal);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(newItem);

			newItem = await _soalService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_soalService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateAsync(string id, [FromBody] SoalDTO soal, CancellationToken cancellationToken)
		{
			InitUserInfo();
			//if (!AllowUpdate) return ValidationProblem();
			var specFilter = new SoalFilterSpecification(soal.Id, true);
			//var rowCount = await _soalService.CountAsync(specFilter, cancellationToken);
			//if (rowCount == 0)
			//	throw new EntityNotFoundException(nameof(Soal), soal.Id);

			// bind to old item


			var objItem = _mapper.Map<Soal>(soal);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(objItem);

			var result = await _soalService.UpdateAsync(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_soalService.Errors);
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
			//if (!AllowDelete) return ValidationProblem();
			// validate if data exists
			//var itemToDelete = await _soalService.GetByIdAsync(id, cancellationToken);
			//if (itemToDelete == null)
			//	throw new EntityNotFoundException(nameof(Soal), id);
			var itemToDelete = new Soal() { Id = id };

			// delete data
			var result = await _soalService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_soalService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#endregion

		#region Recovery Record Controller

		#region appgen: create draft
		//[HttpPost]
		//[Route("create")]
		//public async Task<ActionResult> CreateDraftAsync(CancellationToken cancellationToken)
		//{
		//	InitUserInfo();
		//	if (!AllowCreate) return ValidationProblem();
		//	var newItem = await _soalService.CreateDraft(cancellationToken);
		//	if (newItem == null)
		//	{
		//		AssignToModelState(_soalService.Errors);
		//		return ValidationProblem();
		//	}

		//	return Ok(_mapper.Map<SoalDTO>(newItem));
		//}
		#endregion

		#region appgen: edit draft
		
		//[HttpGet]
		//[Route("edit/{id}")]
		//public async Task<ActionResult> CreateEditDraftAsync(string id, CancellationToken cancellationToken)
		//{
		//	InitUserInfo();
		//	if (!AllowUpdate) return ValidationProblem();
		//	var editItem = await _soalService.CreateEditDraft(id, cancellationToken);
		//	if (editItem == null)
		//		return NotFound();

		//	return Ok(_mapper.Map<SoalDTO>(editItem));
		//}
		
		#endregion

		#region appgen: update patch
		//[HttpPut]
		//[Route("update/{id}")]
		//public async Task<ActionResult> UpdateDraftAsync(string id, [FromBody] SoalDTO soalDto, CancellationToken cancellationToken = default)
		//{
		//	InitUserInfo();
		//	if (!AllowCreate && !AllowUpdate) return ValidationProblem();
		//	var specFilter = new SoalFilterSpecification(soalDto.Id, true);
		//	var rowCount = await _soalService.CountAsync(specFilter, cancellationToken);
		//	if (rowCount == 0)
		//		throw new EntityNotFoundException(nameof(Soal), soalDto.Id);

		//	var soal = _mapper.Map<Soal>(soalDto);
		//	var result = await _soalService.PatchDraft(soal, cancellationToken);
		//	if (!result)
		//	{
		//		AssignToModelState(_soalService.Errors);
		//		return ValidationProblem();
		//	}

		//	return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		//}
		#endregion

		#region appgen: commit changes
		
		//[HttpPut]
		//[Route("commit/{id}")]
		//public async Task<ActionResult> CommitDraftAsync(string id, CancellationToken cancellationToken)
		//{
		//	InitUserInfo();
		//	if (!AllowCreate && !AllowUpdate) return ValidationProblem();
		//	var mainRecordId = await _soalService.CommitDraft(id, cancellationToken);
		//	if (string.IsNullOrEmpty(mainRecordId))
		//	{
		//		AssignToModelState(_soalService.Errors);
		//		return ValidationProblem();
		//	}

		//	return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		//}
		
		#endregion

		#region appgen: discard changes
		//[HttpDelete]
		//[Route("discard/{id}")]
		//public async Task<IActionResult> DiscardAsync(string id, CancellationToken cancellationToken)
		//{
		//	InitUserInfo();
		//	var result = await _soalService.DiscardDraft(id, cancellationToken);
		//	if (!result)
		//	{
		//		AssignToModelState(_soalService.Errors);
		//		return ValidationProblem();
		//	}

		//	return NoContent();
		//}
		#endregion

		#region appgen: get draft list
		
		//[HttpGet]
		//[Route("draftlist")]
		//public async Task<IActionResult> DraftListAsync(CancellationToken cancellationToken)
		//{
		//	InitUserInfo();
		//	if (!AllowRead) return ValidationProblem();
		//	var draftList = await _soalService.GetDraftList(cancellationToken);
		//	return Ok(draftList);
		//}
		
		#endregion

		#region appgen: get current editor
		
		//[HttpGet]
		//[Route("currenteditor/{id}")]
		//public async Task<IActionResult> CurrentEditorAsync(string id, CancellationToken cancellationToken)
		//{
		//	InitUserInfo();
		//	if (!AllowRead) return ValidationProblem();
		//	var currentEditors = await _soalService.GetCurrentEditors(id, cancellationToken);
		//	return Ok(currentEditors);
		//}
		
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
			var item = await _soalService.GetByIdAsync(id, cancellationToken);
			var result = _soalService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_soalService.Errors);
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
			string key = await _soalService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>()
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
		public async Task<ActionResult<IEnumerable<SoalDTO>>> UploadAsync(
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

			var result = await _soalService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_soalService.Errors);
				return ValidationProblem();
			}
			
			List<SoalDTO> resultDto = result.Select(_mapper.Map<SoalDTO>).ToList();
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
			var result = await _soalService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_soalService.Errors);
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
			var result = await _soalService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "Soal.xlsx";
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
			Dictionary<string, List<string>> filterParams = new Dictionary<string, List<string>>();
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

			#region Konten
			List<string> konten = (filterParams.ContainsKey("konten") ? filterParams["konten"] : null);
			#endregion



			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);

			string key = await _soalService.GenerateExcelBackgroundProcess(excelFile, 
				id, konten, 
				exact, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>() { {"id", key} };
			return Ok(result);
/*
			string generatedFilename = await _soalService.GenerateExcel(excelFile, null,
				id, konten, 
				exact, cancellationToken);
			fileName = "Soal.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);
*/
		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private SoalFilter GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0)
		{
			Dictionary<string, List<string>> filterParams = CleanFilter(filters);

			var soalFilter = new SoalFilter(pageSize, pageIndex);

			#region Id
			string id = (filterParams.ContainsKey("id") ? filterParams["id"][0] : string.Empty);
			if (!exact.ContainsKey("id") || (exact.ContainsKey("id") && exact["id"] == 0))
            {
				soalFilter.IdEqual = id;
			} else
            {
				soalFilter.IdContain = id;
			}
			#endregion

			#region Konten
			List<string> Kontens = (filterParams.ContainsKey("konten") ? filterParams["konten"] : null);
			if (!exact.ContainsKey("konten") || (exact.ContainsKey("konten") && exact["konten"] == 0))
			{
				soalFilter.KontenContains = Kontens;
			}
			else
			{
				soalFilter.KontenEquals = Kontens;
			}
			#endregion

			
			// RECOVERY FILTER	
			soalFilter.MainRecordId = (filterParams.ContainsKey("mainRecordId") ? filterParams["mainRecordId"][0] : string.Empty);

			return soalFilter;
		}
		#endregion

		#region appgen: generate sorting
		private List<SortingInformation<Soal>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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
		
		private List<SortingInformation<Soal>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<Soal>> sortingSpec = new List<SortingInformation<Soal>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<Soal>(p => p.Id, sortingOrder));
						break;
					case "konten":
						sortingSpec.Add(new SortingInformation<Soal>(p => p.Konten, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<Soal>(p => p.CreatedDate, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
		#endregion


		#region appgen: clean reference object
		private void CleanReferenceObject(Soal entity)
		{


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_soalService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
