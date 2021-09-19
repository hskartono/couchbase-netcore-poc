using Audit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.ApplicationCore.Specifications;
using AppCoreApi.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi
{
	[ApiController]
	public class BaseAPIController : ControllerBase
	{
		protected string _userName;
		protected UserInfo _user;
		protected int _pageSize;
		private readonly IUserInfoService _userInfoService;
		protected Dictionary<string, RoleDetail> _role = new();
		protected Dictionary<string, Role> _roleConfig;
		protected string _functionId;

		protected const string ERR_NOT_ALLOWED= "Access denied.";

		public string UserName { get { return _userName; } set { _userName = value; } } 

		public BaseAPIController()
		{
			LoadDefaultPagingSize();
		}

		public BaseAPIController(string functionId)
		{
			_functionId = functionId;
			LoadDefaultPagingSize();
		}

		public BaseAPIController(IUserInfoService userInfoService, string functionId)
		{
			_userInfoService = userInfoService;
			_functionId = functionId;
			LoadDefaultPagingSize();
		}

		protected void LoadIdentity()
		{
			if(!string.IsNullOrEmpty(_userName) && _userInfoService != null)
			{
				var filterSpec = new UserInfoFilterSpecification(_userName, string.Empty, string.Empty);
				_user = _userInfoService.FirstOrDefaultAsync(filterSpec).Result;

				var roleService = new RoleService(_userInfoService.UnitOfWork);
				var role = roleService.GetUserRole(_userName).Result;
				if(role != null)
				{
					_role.Clear();
					foreach(var item in role.RoleDetails)
					{
						if (_role.ContainsKey(item.FunctionInfoId)) continue;
						_role.Add(item.FunctionInfoId, item);
					}

					_roleConfig = roleService.GetUserRoles(_userName).Result;
				}
			}
		}

		protected void LoadDefaultPagingSize()
		{
			_pageSize = 10;
		}

		protected void AssignToModelState(IEnumerable<string> errors)
		{
			if (errors == null) return;
			int index = 1;
			foreach(var err in errors)
			{
				ModelState.AddModelError(index.ToString(), err);
				//ModelState.AddModelError(Guid.NewGuid().ToString(), err);
				index++;
			}
		}

		private bool CheckPrivilege(bool privilege)
		{
			if (string.IsNullOrEmpty(_functionId) || !_role.ContainsKey(_functionId) || !privilege)
			{
				ModelState.AddModelError("Privilege", ERR_NOT_ALLOWED);
				return false;
			}

			return privilege;
		}

		protected bool AllowCreate
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowCreate);
			}
		}

		protected bool AllowRead
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowRead);
			}
		}

		protected bool AllowUpdate
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowUpdate);
			}
		}

		protected bool AllowDelete
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowDelete);
			}
		}

		protected bool AllowDownload
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowDownload);
			}
		}

		protected bool AllowPrint
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowPrint);
			}
		}

		protected bool AllowUpload
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) && _role[_functionId].AllowUpload);
			}
		}

		protected static Dictionary<string, List<string>> CleanFilter(Dictionary<string, Dictionary<string, List<string>>> filters)
		{
			if (filters == null) throw new ArgumentNullException(nameof(filters));

			if (filters.ContainsKey("exact")) filters.Remove("exact");
			if (filters.ContainsKey("sorting")) filters.Remove("sorting");
			if (filters.ContainsKey("pageSize")) filters.Remove("pageSize");
			if (filters.ContainsKey("pageIndex")) filters.Remove("pageIndex");

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

			return filterParams;
		}
	}
}
