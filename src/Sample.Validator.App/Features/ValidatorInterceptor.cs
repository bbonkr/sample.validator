using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Sample.Validator.App.Models;

using System.Linq;
using System.Net;
using System.Text.Json.Serialization;

namespace Sample.Validator.App.Features
{
    public class ValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            var statusCode = HttpStatusCode.BadRequest;

            if (!result.IsValid)
            {
                foreach (var parameter in actionContext.ActionDescriptor.Parameters)
                {
                    var properties = parameter.ParameterType.GetMembers();

                    if (properties != null && properties.Length > 0)
                    {
                        foreach (var property in properties.Where(x => x.MemberType == System.Reflection.MemberTypes.Property))
                        {
                            if (property.CustomAttributes != null && property.CustomAttributes.Count() > 0)
                            {
                                var hasJsonIgnore = property.CustomAttributes.Where(a => a.AttributeType == typeof(JsonIgnoreAttribute)).Any();
                                if (hasJsonIgnore)
                                {
                                    var errorItems = result.Errors.Where(x => x.PropertyName == property.Name && new[] { "NotNullValidator", "NotEmptyValidator" }.Contains(x.ErrorCode)).ToList();

                                    foreach (var errorItem in errorItems)
                                    {
                                        result.Errors.Remove(errorItem);
                                    }
                                }
                            }
                        }
                    }
                }

                if (!result.IsValid)
                {
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
            }

            return result;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }
    }
}
