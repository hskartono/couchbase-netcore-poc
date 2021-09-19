using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Exceptions;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Emails
{
	[Authorize]
	[Route("api/emails")]
	[ApiController]
	public class EmailController : BaseAPIController
	{
		private const string functionId= "email_worker";
		private readonly ILogger<EmailController> _logger;
		private readonly IEmailService _emailService;
		private readonly IMapper _mapper;
		
		public EmailController(
			IEmailService emailService,
			IMapper mapper,
			ILogger<EmailController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_emailService = emailService;
			_emailService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<EmailDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _emailService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _emailService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<EmailDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<EmailDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id:int}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<EmailDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _emailService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(Email), id);
			return _mapper.Map<EmailDTO>(item);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] EmailDTO email, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<Email>(email);
			newItem = await _emailService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_emailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] EmailDTO email, CancellationToken cancellationToken)
		{
			var specFilter = new EmailFilterSpecification(email.Id);
			var rowCount = await _emailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(FunctionInfo), email.Id);

			// bind to old item
			var item = _mapper.Map<Email>(email);

			var result = await _emailService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_emailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
		}

		// DELETE api/v1/[controller]/id
		[HttpDelete]
		[Route("{id:int}")]
		public async Task<IActionResult> DeleteItemAsync(int id, CancellationToken cancellationToken)
		{
			// validate if data exists
			var itemToDelete = await _emailService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(Email), id);

			// delete data
			var result = await _emailService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_emailService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private EmailFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			int? emailStatus = (filter.ContainsKey("emailstatus") ? int.Parse(filter["emailstatus"]) : null);
			string createdById = (filter.ContainsKey("createdbyid") ? filter["createdbyid"] : string.Empty);


			if (pageSize == 0)
				return new EmailFilterSpecification((Email.EmailStatus?)emailStatus, createdById);

			return new EmailFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				emailStatus: (Email.EmailStatus?)emailStatus,
				createdById: createdById
			);
		}

		private List<SortingInformation<Email>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<Email>> sortingSpec = new List<SortingInformation<Email>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<Email>(p => p.Id, sortingOrder));
						break;

					case "subject":
						sortingSpec.Add(new SortingInformation<Email>(p => p.Subject, sortingOrder));
						break;

					case "statusender":
						sortingSpec.Add(new SortingInformation<Email>(p => p.Sender, sortingOrder));
						break;

					case "senddate":
						sortingSpec.Add(new SortingInformation<Email>(p => p.SendDate, sortingOrder));
						break;

					case "status":
						sortingSpec.Add(new SortingInformation<Email>(p => p.Status, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<Email>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}

	}
}
