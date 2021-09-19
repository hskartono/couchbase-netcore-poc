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

namespace AppCoreApi.PublicApi.Features.Kuisioners
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class KuisionerDetailController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "kuisioner_detail";
		private readonly ILogger<KuisionerDetailController> _logger;
		private readonly IKuisionerDetailService _kuisionerDetailService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public KuisionerDetailController(
			IKuisionerDetailService kuisionerDetailService,
			IMapper mapper,
			ILogger<KuisionerDetailController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_kuisionerDetailService = kuisionerDetailService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<KuisionerDetailDTO>>> GetListAsync(
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
			var totalItems = await _kuisionerDetailService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _kuisionerDetailService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<KuisionerDetailDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<KuisionerDetailDTO>));
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
			var items = await _kuisionerDetailService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<KuisionerDetailDTO>> GetIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _kuisionerDetailService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(KuisionerDetail), id);
			return _mapper.Map<KuisionerDetailDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] KuisionerDetailDTO kuisionerDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<KuisionerDetail>(kuisionerDetail);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(newItem);

			newItem = await _kuisionerDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] KuisionerDetailDTO kuisionerDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new KuisionerDetailFilterSpecification(int.Parse(kuisionerDetail.Id), true);
			var rowCount = await _kuisionerDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(KuisionerDetail), kuisionerDetail.Id);

			// bind to old item


			var objItem = _mapper.Map<KuisionerDetail>(kuisionerDetail);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(objItem);

			var result = await _kuisionerDetailService.PatchDraft(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
			var itemToDelete = await _kuisionerDetailService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(KuisionerDetail), id);

			// delete data
			var result = await _kuisionerDetailService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
		public async Task<ActionResult> CreateDraftAsync([FromBody] KuisionerDetailDTO kuisionerDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			var newItem = _mapper.Map<KuisionerDetail>(kuisionerDetail);
			 newItem = await _kuisionerDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<KuisionerDetailDTO>(newItem));
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
			var editItem = await _kuisionerDetailService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<KuisionerDetailDTO>(editItem));
		}
		*/
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(int id, [FromBody] KuisionerDetailDTO kuisionerDetailDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new KuisionerDetailFilterSpecification(int.Parse(kuisionerDetailDto.Id), true);
			var rowCount = await _kuisionerDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(KuisionerDetail), kuisionerDetailDto.Id);

			var kuisionerDetail = _mapper.Map<KuisionerDetail>(kuisionerDetailDto);
			var result = await _kuisionerDetailService.PatchDraft(kuisionerDetail, cancellationToken);
			if (!result)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
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
			var mainRecordId = await _kuisionerDetailService.CommitDraft(id, cancellationToken);
			if (mainRecordId <= 0)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
			var result = await _kuisionerDetailService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
			var draftList = await _kuisionerDetailService.GetDraftList(cancellationToken);
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
			var currentEditors = await _kuisionerDetailService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
		}
		*/
		#endregion

		#region appgen: replace async
		[HttpPost]
		[Route("replace")]
		public async Task<ActionResult> ReplaceAsync(
			[FromBody] List<KuisionerDetail> kuisionerDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = kuisionerDetails.Select(_mapper.Map<KuisionerDetail>).ToList();

			foreach (var item in newItems)
				CleanReferenceObject(item);

			newItems = await _kuisionerDetailService.ReplaceDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
				return ValidationProblem();
			}

			return Ok();
		}
		#endregion

		#region appgen: add async
		[HttpPost]
		[Route("add")]
		public async Task<ActionResult> AddAsync(
			[FromBody] List<KuisionerDetail> kuisionerDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = kuisionerDetails.Select(_mapper.Map<KuisionerDetail>).ToList();

			foreach(var item in newItems)
				CleanReferenceObject(item);

			newItems = await _kuisionerDetailService.AddDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
			var item = await _kuisionerDetailService.GetByIdAsync(id, cancellationToken);
			var result = _kuisionerDetailService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
			string key = await _kuisionerDetailService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
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
		public async Task<ActionResult<IEnumerable<KuisionerDetailDTO>>> UploadAsync(
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

			var result = await _kuisionerDetailService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
				return ValidationProblem();
			}
			
			List<KuisionerDetailDTO> resultDto = result.Select(_mapper.Map<KuisionerDetailDTO>).ToList();
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
			var result = await _kuisionerDetailService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_kuisionerDetailService.Errors);
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
			var result = await _kuisionerDetailService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "KuisionerDetail.xlsx";
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
			int? id = (filterParams.ContainsKey("id") ? int.Parse(filterParams["id"][0]) : null);
			#endregion

			#region Kuisioner
			List<int> kuisionerId = null;
			if (filterParams.ContainsKey("kuisionerId")) 
			{
				kuisionerId = new List<int>();
				foreach(var item in filterParams["kuisionerId"])
				{
					var data = int.Parse(item);
					kuisionerId.Add(data);
				}
			}
			#endregion

			#region Soal
			List<string> soalId = (filterParams.ContainsKey("soalId") ? filterParams["soalId"] : null);
			#endregion

			#region Konten Soal
			List<string> kontenSoal = (filterParams.ContainsKey("kontenSoal") ? filterParams["kontenSoal"] : null);
			#endregion

			#region Pilihan1
			List<string> pilihan1 = (filterParams.ContainsKey("pilihan1") ? filterParams["pilihan1"] : null);
			#endregion

			#region P Ilihan2
			List<string> pIlihan2 = (filterParams.ContainsKey("pIlihan2") ? filterParams["pIlihan2"] : null);
			#endregion

			#region Pilihan3
			List<string> pilihan3 = (filterParams.ContainsKey("pilihan3") ? filterParams["pilihan3"] : null);
			#endregion

			#region Kunci Jawaban
			List<int> kunciJawaban = null;
			if (filterParams.ContainsKey("kunciJawaban")) 
			{
				kunciJawaban = new List<int>();
				foreach(var item in filterParams["kunciJawaban"])
				{
					var data = int.Parse(item);
					kunciJawaban.Add(data);
				}
			}
			#endregion



			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);
/*
			string key = await _kuisionerDetailService.GenerateExcelBackgroundProcess(excelFile, 
				id, kuisionerId, soalId, kontenSoal, pilihan1, pIlihan2, pilihan3, kunciJawaban, 
				exact, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>() { {"id", key} };
			return Ok(result);
*/
			string generatedFilename = await _kuisionerDetailService.GenerateExcel(excelFile, null,
				id, kuisionerId, soalId, kontenSoal, pilihan1, pIlihan2, pilihan3, kunciJawaban, 
				exact, cancellationToken);
			fileName = "KuisionerDetail.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);

		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private KuisionerDetailFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<KuisionerDetail>> sorting = null)
		{
			Dictionary<string, List<string>> filterParams = new Dictionary<string, List<string>>();
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

			#region Kuisioner
			List<int> kuisionerId = null;
			if (filterParams.ContainsKey("kuisionerId")) 
			{
				kuisionerId = new List<int>();
				foreach(var item in filterParams["kuisionerId"])
				{
					var data = int.Parse(item);
					kuisionerId.Add(data);
				}
			}
			#endregion

			#region Soal
			List<string> soalId = (filterParams.ContainsKey("soalId") ? filterParams["soalId"] : null);
			#endregion

			#region Konten Soal
			List<string> kontenSoal = (filterParams.ContainsKey("kontenSoal") ? filterParams["kontenSoal"] : null);
			#endregion

			#region Pilihan1
			List<string> pilihan1 = (filterParams.ContainsKey("pilihan1") ? filterParams["pilihan1"] : null);
			#endregion

			#region P Ilihan2
			List<string> pIlihan2 = (filterParams.ContainsKey("pIlihan2") ? filterParams["pIlihan2"] : null);
			#endregion

			#region Pilihan3
			List<string> pilihan3 = (filterParams.ContainsKey("pilihan3") ? filterParams["pilihan3"] : null);
			#endregion

			#region Kunci Jawaban
			List<int> kunciJawaban = null;
			if (filterParams.ContainsKey("kunciJawaban")) 
			{
				kunciJawaban = new List<int>();
				foreach(var item in filterParams["kunciJawaban"])
				{
					var data = int.Parse(item);
					kunciJawaban.Add(data);
				}
			}
			#endregion


			
			// RECOVERY FILTER	
			int? mainRecordId = (filterParams.ContainsKey("mainRecordId") ? int.Parse(filterParams["mainRecordId"][0]) : null);
			bool mainRecordIsNull = (filterParams.ContainsKey("mainRecordIsNull") ? bool.Parse(filterParams["mainRecordIsNull"][0]) : false);
			string recordEditedBy = (filterParams.ContainsKey("recordEditedBy") ? filterParams["recordEditedBy"][0] : null);
			if (filterParams.ContainsKey("draftFromUpload"))
			{
				if (filterParams["draftFromUpload"][0] == "1") 
					filterParams["draftFromUpload"][0] = "true";
				else
					filterParams["draftFromUpload"][0] = "false";
			}
			bool? draftFromUpload = (filterParams.ContainsKey("draftFromUpload") ? bool.Parse(filterParams["draftFromUpload"][0]) : false);
			if (filterParams.ContainsKey("draftMode"))
			{
				if (filterParams["draftMode"][0] == "true") filterParams["draftMode"][0] = "1";
				if (filterParams["draftMode"][0] == "false") filterParams["draftMode"][0] = "0";
			}
			int draftMode = (filterParams.ContainsKey("draftMode") ? int.Parse(filterParams["draftMode"][0]) : -1);

			if (pageSize == 0)
			{
				return new KuisionerDetailFilterSpecification(exact)
				{

					Id = id, 
					KuisionerIds = kuisionerId, 
					SoalIds = soalId, 
					KontenSoals = kontenSoal, 
					Pilihan1s = pilihan1, 
					PIlihan2s = pIlihan2, 
					Pilihan3s = pilihan3, 
					KunciJawabans = kunciJawaban,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new KuisionerDetailFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					KuisionerIds = kuisionerId, 
					SoalIds = soalId, 
					KontenSoals = kontenSoal, 
					Pilihan1s = pilihan1, 
					PIlihan2s = pIlihan2, 
					Pilihan3s = pilihan3, 
					KunciJawabans = kunciJawaban,

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
		private List<SortingInformation<KuisionerDetail>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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
		
		private List<SortingInformation<KuisionerDetail>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<KuisionerDetail>> sortingSpec = new List<SortingInformation<KuisionerDetail>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.Id, sortingOrder));
						break;
					case "kuisioner":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.Kuisioner.Judul, sortingOrder));
						break;
					case "soal":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.Soal.Konten, sortingOrder));
						break;
					case "kontensoal":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.KontenSoal, sortingOrder));
						break;
					case "pilihan1":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.Pilihan1, sortingOrder));
						break;
					case "pilihan2":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.PIlihan2, sortingOrder));
						break;
					case "pilihan3":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.Pilihan3, sortingOrder));
						break;
					case "kuncijawaban":
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.KunciJawaban, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<KuisionerDetail>(p => p.Id, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
		#endregion

		#region appgen: clean filter parameters
		private void CleanFilter(Dictionary<string, Dictionary<string, List<string>>> filter)
		{
			if (filter.ContainsKey("exact")) filter.Remove("exact");
			if (filter.ContainsKey("sorting")) filter.Remove("sorting");
			if (filter.ContainsKey("pageSize")) filter.Remove("pageSize");
			if (filter.ContainsKey("pageIndex")) filter.Remove("pageIndex");
		}
		#endregion

		#region appgen: clean reference object
		private void CleanReferenceObject(KuisionerDetail entity)
		{
			entity.Kuisioner = null;
			entity.Soal = null;


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_kuisionerDetailService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
