using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Exceptions;
using AppCoreApi.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace AppCoreApi.PublicApi.Filters
{
	public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
		//private readonly IAppLogger<ApiExceptionFilterAttribute> _logger;

		public ApiExceptionFilterAttribute()
		{
			// _logger = logger;
			_exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
			{
				{typeof (EntityNotFoundException), HandleNotFoundException },
				{ typeof(EntityAlreadyExistsException), HandleAlreadyExistsException },
			};
		}

		public override void OnException(ExceptionContext context)
		{
			HandleException(context);
			base.OnException(context);
		}

		private void HandleException(ExceptionContext context)
		{
			// Log.Error(context.Exception, "Handling exception:");
			NLog.LogManager.GetCurrentClassLogger().Warn(context.Exception, "Handling Exception:");

			Type type = context.Exception.GetType();
			if (_exceptionHandlers.ContainsKey(type))
			{
				_exceptionHandlers[type].Invoke(context);
				return;
			}

			if (!context.ModelState.IsValid)
			{
                HandleInvalidModelStateException(context);
				return;
			}

            HandleUnknownException(context);
		}

		private static void HandleUnknownException(ExceptionContext context)
		{
			ProblemDetails details = new()
            {
				Status = StatusCodes.Status500InternalServerError,
				Title = context.Exception.Message, // "An error occurred while processing your request.",
				Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
			};

			if (context.Exception.InnerException != null)
				details.Detail = context.Exception.InnerException.Message;

			context.Result = new ObjectResult(details)
			{
				StatusCode = StatusCodes.Status500InternalServerError
			};

			context.ExceptionHandled = true;
		}

		private static void HandleInvalidModelStateException(ExceptionContext context)
		{
			ValidationProblemDetails details = new(context.ModelState)
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
			};

			context.Result = new BadRequestObjectResult(details);

			context.ExceptionHandled = true;
		}

		private void HandleNotFoundException(ExceptionContext context)
		{
			EntityNotFoundException exception = context.Exception as EntityNotFoundException;

			ProblemDetails details = new()
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
				Title = "The specified resource was not found.",
				Detail = exception.Message
			};

			context.Result = new NotFoundObjectResult(details);

			context.ExceptionHandled = true;
		}

		private void HandleAlreadyExistsException(ExceptionContext context)
		{
			EntityAlreadyExistsException exception = context.Exception as EntityAlreadyExistsException;

			ProblemDetails details = new()
			{
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
				Title = "The specified resource already exists.",
				Detail = exception.Message
			};

			//context.Result = new NotFoundObjectResult(details);
			context.Result = new ConflictObjectResult(details);

			context.ExceptionHandled = true;
		}
	}
}
