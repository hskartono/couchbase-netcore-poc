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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.UserRoles
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class UserRoleController : BaseAPIController
	{
		private const string functionId = "user_role";
		private readonly ILogger<UserRoleController> _logger;
		private readonly IUserRoleService _userRoleService;
		private readonly IMapper _mapper;

		public UserRoleController(
			IUserRoleService userRoleService,
			IMapper mapper,
			ILogger<UserRoleController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_userRoleService = userRoleService;
			_userRoleService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<UserRoleDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _userRoleService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _userRoleService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<UserRoleDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<UserRoleDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id:int}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<UserRoleDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _userRoleService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserRole), id);
			return _mapper.Map<UserRoleDTO>(item);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] UserRoleDTO userRoleDTO, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<UserRole>(userRoleDTO);
			newItem = await _userRoleService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_userRoleService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] UserRoleDTO userRoleDTO, CancellationToken cancellationToken)
		{
			var specFilter = new UserRoleFilterSpecification(userRoleDTO.Id.Value);
			var rowCount = await _userRoleService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(UserRole), userRoleDTO.Id);

			// bind to old item
			var item = _mapper.Map<UserRole>(userRoleDTO);

			var result = await _userRoleService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_userRoleService.Errors);
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
			var itemToDelete = await _userRoleService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(UserRole), id);

			// delete data
			var result = await _userRoleService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_userRoleService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private UserRoleFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			string userName = (filter.ContainsKey("username") ? filter["username"] : string.Empty);
			string userId = (filter.ContainsKey("userid") ? filter["userid"] : "");

			if (pageSize == 0)
				return new UserRoleFilterSpecification(userId, userName);

			return new UserRoleFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				userName: userName,
				userId: userId
			);
		}

		private List<SortingInformation<UserRole>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<UserRole>> sortingSpec = new List<SortingInformation<UserRole>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<UserRole>(p => p.Id, sortingOrder));
						break;

					case "userinfousername":
						sortingSpec.Add(new SortingInformation<UserRole>(p => p.UserInfo.UserName, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<UserRole>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
	}
}
