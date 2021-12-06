using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Mvc.Filters;

using Sample.Validator.App.Models;
using System.Linq;

namespace Sample.Validator.App.Features
{
    public class ApiExceptionHandlerFilter: ExceptionFilterAttribute, IExceptionFilter, IFilterMetadata, IAsyncExceptionFilter
    {
        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            HandleException(context);

            return Task.CompletedTask;
        }

        private void HandleException(ExceptionContext context)
        {
            ObjectResult actionResult = default;

            IApiResponse responseModel = default;

            var statusCode = HttpStatusCode.InternalServerError;
            var statusCodeValue = (int)statusCode;

            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;
            var instance = context.ActionDescriptor.DisplayName;

            if (context.Exception is ApiException apiException)
            {
                statusCodeValue = apiException.StatusCode;
                responseModel = ApiResponseModelFactory.Create(statusCodeValue, apiException.Message, apiException.Error);
            }

            if (context.Exception is AggregateException aggregateException)
            {
                var innerErrors = aggregateException.InnerExceptions != null && aggregateException.InnerExceptions.Count > 0
                        ? aggregateException.InnerExceptions.Select((inner, index) => new ErrorModel(inner.Message, code: $"Inner Error {index + 1}")).ToList()
                        : new List<ErrorModel>();

                responseModel = ApiResponseModelFactory.Create(
                    statusCodeValue,
                    aggregateException.Message,
                    new ErrorModel(aggregateException.Message, code: $"{(HttpStatusCode)statusCodeValue}", innerErrors: innerErrors));
            }

            if (responseModel == null)
            {
                var innerErrors = context.Exception.InnerException != null
                    ? new List<ErrorModel>{
                        new ErrorModel(context.Exception.InnerException.Message)
                    }
                    : new List<ErrorModel>();


                responseModel = ApiResponseModelFactory.Create(statusCodeValue, context.Exception.Message, new ErrorModel(context.Exception.Message, code: $"{(HttpStatusCode)statusCodeValue}", innerErrors: innerErrors));
            }

            responseModel.Path = path;
            responseModel.Instance = instance;
            responseModel.Method = method;

            actionResult = new ObjectResult(responseModel);

            context.HttpContext.Response.StatusCode = statusCodeValue;
            context.Result = actionResult;
        }
    }
}
