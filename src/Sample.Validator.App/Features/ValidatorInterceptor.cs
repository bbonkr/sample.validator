using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using Sample.Validator.App.Models;

using System.Linq;
using System.Net;

namespace Sample.Validator.App.Features
{
    public class ValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            if (!result.IsValid)
            {
                var statusCode = HttpStatusCode.BadRequest;

                var error = new ErrorModel
                {
                    Message = "Request payload is invalid",
                    Code = statusCode.ToString(),
                    InnerErrors = result.Errors.Select(x => new ErrorModel
                    {
                        Message = x.ErrorMessage,
                        Code = x.ErrorCode,
                        Reference = x.PropertyName
                    }),
                };

                throw new ApiException(statusCode, error);
            }

            return result;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            

            return commonContext;
        }
    }
}
