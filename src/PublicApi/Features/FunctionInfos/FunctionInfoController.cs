using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

namespace AppCoreApi.PublicApi.Features.FunctionInfos
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class FunctionInfoController : BaseAPIController
	{
		private const string functionId = "function_info";
		private readonly ILogger<FunctionInfoController> _logger;
		private readonly IFunctionInfoService _functionInfoService;
		private readonly IMapper _mapper;

		public FunctionInfoController(
			IFunctionInfoService functionInfoService,
			IMapper mapper,
			ILogger<FunctionInfoController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_functionInfoService = functionInfoService;
			_functionInfoService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<FunctionInfoDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			CleanFilter(filter);
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _functionInfoService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _functionInfoService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<FunctionInfoDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<FunctionInfoDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<FunctionInfoDTO>> ItemByIdAsync(string id, CancellationToken cancellationToken)
		{
			var item = await _functionInfoService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(FunctionInfo), id);
			return _mapper.Map<FunctionInfoDTO>(item);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] FunctionInfoDTO email, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<FunctionInfo>(email);
			newItem = await _functionInfoService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_functionInfoService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateItemAsync(string id, [FromBody] FunctionInfoDTO functionInfoDTO, CancellationToken cancellationToken)
		{
			var specFilter = new FunctionInfoFilterSpecification(functionInfoDTO.Name, null);
			var rowCount = await _functionInfoService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(FunctionInfo), functionInfoDTO.Id);

			// bind to old item
			var item = _mapper.Map<FunctionInfo>(functionInfoDTO);

			var result = await _functionInfoService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_functionInfoService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
		}

		// DELETE api/v1/[controller]/id
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteItemAsync(string id, CancellationToken cancellationToken)
		{
			// validate if data exists
			var itemToDelete = await _functionInfoService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(FunctionInfo), id);

			// delete data
			var result = await _functionInfoService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_functionInfoService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private FunctionInfoFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, int pageSize = 0, int pageIndex = 0)
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

			string name = (filterParams.ContainsKey("name") ? filterParams["name"][0] : string.Empty);
			string uri = (filterParams.ContainsKey("uri") ? filterParams["uri"][0] : string.Empty);
			string iconName = (filterParams.ContainsKey("iconName") ? filterParams["iconName"][0] : string.Empty);
			bool? isenabeld = (filterParams.ContainsKey("isEnabled") ? (filterParams["isEnabled"][0] == "true") : null);
			int? moduleInfoId = null;
            if (filterParams.ContainsKey("moduleInfoId"))
            {
				int _out=0;
				if(int.TryParse(filterParams["moduleInfoId"][0], out _out))
                {
					moduleInfoId = _out;
				}
			}
			

			if (pageSize == 0)
            {
				return new FunctionInfoFilterSpecification()
				{
					NameContains = name,
					IsEnabled = isenabeld,
					ModuleInfoId = moduleInfoId,
					uriContains = uri,
					iconNameContains=iconName,
				}.BuildSpecification(true);
			}

			return new FunctionInfoFilterSpecification(
				pageSize * pageIndex,
				pageSize
			)
			{
				NameContains = name,
				IsEnabled = isenabeld,
				ModuleInfoId = moduleInfoId,
				uriContains = uri,
				iconNameContains = iconName,
			}.BuildSpecification(true);
		}

		private List<SortingInformation<FunctionInfo>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<FunctionInfo>> sortingSpec = new List<SortingInformation<FunctionInfo>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<FunctionInfo>(p => p.Id, sortingOrder));
						break;

					case "name":
						sortingSpec.Add(new SortingInformation<FunctionInfo>(p => p.Name, sortingOrder));
						break;

					case "isenabled":
						sortingSpec.Add(new SortingInformation<FunctionInfo>(p => p.IsEnabled, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<FunctionInfo>(p => p.CreatedDate, SortingType.Descending));
						break;
				}
			}

			return sortingSpec;
		}

		private void CleanFilter(Dictionary<string, Dictionary<string, List<string>>> filter)
		{
			if (filter.ContainsKey("exact")) filter.Remove("exact");
			if (filter.ContainsKey("sorting")) filter.Remove("sorting");
			if (filter.ContainsKey("pageSize")) filter.Remove("pageSize");
			if (filter.ContainsKey("pageIndex")) filter.Remove("pageIndex");
		}

		private void InitUserInfo()
		{
			LoadIdentity();
			_functionInfoService.UserInfo = _user;
		}
	}
}
