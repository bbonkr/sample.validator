using System;
using System.Collections.Generic;

using FluentValidation;

using MediatR;

using Sample.Validator.App.Models;

namespace Sample.Validator.App.Cqrs.Samples.Queries
{
    public class GetSamplesQuery : IRequest<IEnumerable<WeatherForecast>>
    {
        public GetSamplesQuery(GetSamplesQueryFilter filter)
        {
            Filter = filter;
        }

        public GetSamplesQueryFilter Filter { get; }
    }

    public class GetSamplesQueryFilter
    {
        public Guid Id { get; set; }
    }

    public class GetSamplesQueryFilterValidator : AbstractValidator<GetSamplesQueryFilter>
    {
        public GetSamplesQueryFilterValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("Id is required.");
        }
    }
}
