using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using Sample.Validator.App.Cqrs.Samples.Commands;
using Sample.Validator.App.Models;

namespace Sample.Validator.App.Cqrs.Samples.Queries
{
    public class GetSamplesQueryHandler : IRequestHandler<GetSamplesQuery, IEnumerable<WeatherForecast>>
    {
        public GetSamplesQueryHandler(IMediator mediator, ILogger<GetSamplesQueryHandler> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<IEnumerable<WeatherForecast>> Handle(GetSamplesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetSamplesQuery executed!");

            var resultforCreateSampleCommand = await mediator.Send(new CreateSampleCommand(new CreateSampleCommandPayload
            {
                //Id = request.Filter.Id,
                // If command validator verifies, use below line. (It uses nested validator)
                Id = null,
            }));

            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
           .ToArray();

            return result.AsEnumerable();
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IMediator mediator;
        private readonly ILogger logger;
    }
}
