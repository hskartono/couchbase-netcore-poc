using Audit.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppCoreApi.PublicApi.Filters
{
	public class LoginFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var userIdentifier = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userIdentifier == null) return;
			((BaseAPIController)context.Controller).UserName = userIdentifier;
			Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
			{
				//scope.SetCustomField("User", userIdentifier);
				scope.Event.Environment.UserName = userIdentifier;
				//Or: scope.Event.Environment.UserName = ...
			});
			//base.OnActionExecuting(context);
		}
	}
}
