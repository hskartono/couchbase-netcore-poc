using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Attachments
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class AttachmentController : BaseAPIController
	{
		private const string functionId = "attachment";
		private readonly ILogger<AttachmentController> _logger;
		private readonly IAttachmentService _attachmentService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		public AttachmentController(
			IAttachmentService attachmentService,
			IMapper mapper,
			ILogger<AttachmentController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_attachmentService = attachmentService;
			_attachmentService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<AttachmentDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _attachmentService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _attachmentService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<AttachmentDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<AttachmentDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<AttachmentDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _attachmentService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(FunctionInfo), id);

			var services = this.HttpContext.RequestServices;
			var uriComposer = (IUriComposer)services.GetService(typeof(IUriComposer));
			item.ComposeDownloadUrl(
				uriComposer.ComposeDownloadUri(
					Path.GetFileName(item.SavedFileName)
				)
			);

			return _mapper.Map<AttachmentDTO>(item);
		}

		[HttpGet]
		[Route("download/{id}")]
		public async Task<IActionResult> DownloadByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _attachmentService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(FunctionInfo), id);

			// force download here
			var fileName = item.OriginalFileName;
			var savedFileName = Path.GetFileName(item.SavedFileName);
			var contentType = "application/octet-stream";
			new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
			var fileLocation = _uriComposer.ComposeUploadPath(savedFileName);
			byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
			return File(fileBytes, (contentType ?? "application/octet-stream"), fileName);
		}

		// POST api/v1/[controller]
		[HttpPost]
		[Route("upload")]
		public async Task<ActionResult> CreateItemAsync(IFormFile file, CancellationToken cancellationToken)
		{
			var fileExt = Path.GetExtension(file.FileName);
			var fileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + fileExt;
			var destinationFile = _uriComposer.ComposeUploadPath(fileName);
			using (var stream = System.IO.File.Create(destinationFile))
			{
				await file.CopyToAsync(stream, cancellationToken);
			}

			var newItem = new Attachment(fileName, fileName, fileExt, file.Length);
			newItem = await _attachmentService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_attachmentService.Errors);
				return ValidationProblem();
			}

			var dto = _mapper.Map<AttachmentDTO>(newItem);
			return Ok(dto);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateItemAsync(IFormFile file, int id, CancellationToken cancellationToken)
		{
			var item = await _attachmentService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(FunctionInfo), id);

			var fileExt = Path.GetExtension(file.FileName);
			var fileName = Path.GetFileName(file.FileName);
			var filePath = _uriComposer.ComposeUploadPath(DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + fileExt);
			using (var stream = System.IO.File.Create(filePath))
			{
				await file.CopyToAsync(stream, cancellationToken);
			}

			item.FileExtension = fileExt;
			item.OriginalFileName = fileName;
			item.SavedFileName = filePath;
			item.FileSize = file.Length;
			var result = await _attachmentService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_attachmentService.Errors);
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
			var itemToDelete = await _attachmentService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(FunctionInfo), id);

			// delete data
			var result = await _attachmentService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_attachmentService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private AttachmentFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filterParam, 
			int pageSize = 0, int pageIndex = 0)
		{
			Dictionary<string, List<string>> filter = new Dictionary<string, List<string>>();
			foreach (string fieldName in filterParam.Keys)
			{
				if (filterParam[fieldName].Count <= 0) continue;
				filter.Add(fieldName, new List<string>());
				foreach (var itemValue in filterParam[fieldName][""])
				{
					filter[fieldName].Add(itemValue);
				}
			}

			List<string> filenames = (filter.ContainsKey("filename") ? filter["filename"] : null);
			List<string> fileextensions = (filter.ContainsKey("filename") ? filter["filename"] : null);

			if (pageSize == 0)
				return new AttachmentFilterSpecification(filenames, fileextensions);

			return new AttachmentFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				fileNames: filenames,
				fileExtensions: fileextensions
			);
		}

		private List<SortingInformation<Attachment>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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

		private List<SortingInformation<Attachment>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<Attachment>> sortingSpec = new List<SortingInformation<Attachment>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<Attachment>(p => p.Id, sortingOrder));
						break;

					case "name":
						sortingSpec.Add(new SortingInformation<Attachment>(p => p.OriginalFileName, sortingOrder));
						break;

					case "isenabled":
						sortingSpec.Add(new SortingInformation<Attachment>(p => p.FileExtension, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<Attachment>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}

		private void CleanFilter(Dictionary<string, Dictionary<string, List<string>>> filter)
		{
			if (filter.ContainsKey("sorting")) filter.Remove("sorting");
			if (filter.ContainsKey("pageSize")) filter.Remove("pageSize");
			if (filter.ContainsKey("pageIndex")) filter.Remove("pageIndex");
		}
	}
}
