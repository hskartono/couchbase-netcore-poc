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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.Roles
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class RoleController : BaseAPIController
	{
		#region Private Variable Declarations

		private const string functionId = "role";
		private readonly ILogger<RoleController> _logger;
		private readonly IRoleService _roleService;
		private readonly IMapper _mapper;

		#endregion

		#region Class Constructor

		public RoleController(
			IRoleService roleService,
			IMapper mapper,
			ILogger<RoleController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_roleService = roleService;
			_mapper = mapper;
			_logger = logger;
		}

		#endregion

		#region CRUD Operations

		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<RoleDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10, 
			[FromQuery] int pageIndex = 0, 
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _roleService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _roleService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<RoleDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<RoleDTO>));
			return Ok(model);
		}

		[HttpGet]
		[Route("{id:int}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<RoleDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var item = await _roleService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(Role), id);
			return _mapper.Map<RoleDTO>(item);
		}

		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] RoleDTO role, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var newItem = _mapper.Map<Role>(role);
			newItem = await _roleService.AddAsync(newItem, cancellationToken);
			if(newItem == null)
			{
				AssignToModelState(_roleService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] RoleDTO role, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var specFilter = new RoleFilterSpecification(role.Id);
			var rowCount = await _roleService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(Role), role.Id);

			// bind to old item
			var item = _mapper.Map<Role>(role);

			var result = await _roleService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleService.Errors);
				return ValidationProblem();
			}
			
			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
		}

		[Route("{id}")]
		[HttpDelete]
		public async Task<IActionResult> DeleteItemAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			// validate if data exists
			var itemToDelete = await _roleService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(Role), id);

			// delete data
			var result = await _roleService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_roleService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		[HttpGet]
		[Route("me")]
		public async Task<ActionResult<RoleDTO>> GetMyRoles(CancellationToken cancellationToken)
		{
			InitUserInfo();
			var myRole = await _roleService.GetUserRole(_userName, cancellationToken);
			return _mapper.Map<RoleDTO>(myRole);
		}

		#endregion

		#region Private Methods

		private RoleFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			string name = (filter.ContainsKey("name") ? filter["name"] : string.Empty);
			string description = (filter.ContainsKey("description") ? filter["description"] : string.Empty);

			if (pageSize == 0)
				return new RoleFilterSpecification(name, description);

			return new RoleFilterSpecification (
				skip: pageSize * pageIndex,
				take: pageSize,
				name: name,
				description: description
			);
		}

		private List<SortingInformation<Role>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<Role>> sortingSpec = new List<SortingInformation<Role>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<Role>(p => p.Id, sortingOrder));
						break;

					case "name":
						sortingSpec.Add(new SortingInformation<Role>(p => p.Name, sortingOrder));
						break;

					case "description":
						sortingSpec.Add(new SortingInformation<Role>(p => p.Description, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<Role>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}

		private void InitUserInfo()
		{
			LoadIdentity();
			_roleService.UserInfo = _user;
		}

		#endregion

		#region GetUserGroup

		[HttpGet]
		[Route("myusergroup")]
		public async Task<ActionResult<RoleDTO>> GetMyUserGroup(CancellationToken cancellationToken)
		{
			InitUserInfo();
			var myUserGroup = await _roleService.GetUserGroup(_userName, cancellationToken);
			return _mapper.Map<RoleDTO>(myUserGroup);
		}
		#endregion
	}
}
