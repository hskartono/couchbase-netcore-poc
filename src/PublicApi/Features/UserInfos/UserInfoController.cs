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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Features.UserInfos
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class UserInfoController : BaseAPIController
	{
		private const string functionId = "user_info";
		private readonly ILogger<UserInfoController> _logger;
		private readonly IUserInfoService _userInfoService;
		private readonly IMapper _mapper;

		public UserInfoController(
			IUserInfoService userInfoService,
			IMapper mapper,
			ILogger<UserInfoController> logger) : base(userInfoService, functionId)
		{
			_userInfoService = userInfoService;
			_userInfoService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<UserInfoDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _userInfoService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _userInfoService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<UserInfoDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<UserInfoDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<UserInfoDTO>> ItemByIdAsync(string id, CancellationToken cancellationToken)
		{
			var item = await _userInfoService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			return _mapper.Map<UserInfoDTO>(item);
		}

		[HttpGet]
		[Route("Me")]
		public async Task<ActionResult<UserInfoDTO>> Me(CancellationToken token = default)
        {
			var user = await _userInfoService.GetByIdAsync(_userName, token);
			if (user == null)
				throw new EntityNotFoundException(nameof(UserInfo));
			return _mapper.Map<UserInfoDTO>(user);
        }

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] UserInfoDTO userInfo, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<UserInfo>(userInfo);
			newItem = await _userInfoService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_userInfoService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.UserName }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] UserInfoDTO userInfo, CancellationToken cancellationToken)
		{
			var specFilter = new UserInfoFilterSpecification(userInfo.UserName);
			var rowCount = await _userInfoService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(userInfo), userInfo.UserName);

			var item = _mapper.Map<UserInfo>(userInfo);

			var result = await _userInfoService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_userInfoService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.UserName }, null);
		}

		// DELETE api/v1/[controller]/id
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteItemAsync(string id, CancellationToken cancellationToken)
		{
			// validate if data exists
			var itemToDelete = await _userInfoService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			// delete data
			var result = await _userInfoService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_userInfoService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private UserInfoFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, int pageSize = 0, int pageIndex = 0)
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

			string userName = (filterParams.ContainsKey("userName") ? filterParams["userName"][0] : string.Empty);
			string firstName = (filterParams.ContainsKey("firstName") ? filterParams["firstName"][0] : string.Empty);
			string lastName = (filterParams.ContainsKey("lastName") ? filterParams["lastName"][0] : string.Empty);

			if (pageSize == 0)
				return new UserInfoFilterSpecification(userName, firstName, lastName);

			return new UserInfoFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				userName: userName,
				firstName: firstName,
				lastName: lastName
			);
		}

		private List<SortingInformation<UserInfo>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<UserInfo>> sortingSpec = new List<SortingInformation<UserInfo>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "username":
						sortingSpec.Add(new SortingInformation<UserInfo>(p => p.UserName, sortingOrder));
						break;

					case "firstname":
						sortingSpec.Add(new SortingInformation<UserInfo>(p => p.FirstName, sortingOrder));
						break;

					case "lastname":
						sortingSpec.Add(new SortingInformation<UserInfo>(p => p.LastName, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<UserInfo>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
	}
}
