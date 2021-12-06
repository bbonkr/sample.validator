using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sample.Validator.App.Cqrs.Samples.Commands;
using Sample.Validator.App.Cqrs.Samples.Queries;
using Sample.Validator.App.Models;

namespace Sample.Validator.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public WeatherForecastController(IMediator mediator, ILogger<WeatherForecastController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }


        /// <summary>
        /// Sample #1
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request
        /// 
        /// ```
        /// GET: /api/WeatherForecast?id=1ac0cf5d-738c-4c9c-9d62-ad07782f8247
        /// ```
        /// </remarks>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get([FromQuery] GetSamplesQueryFilter filter)
        {
            var response = await mediator.Send(new GetSamplesQuery(filter));
         
            return response;
        }

        /// <summary>
        /// Sample #2
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        /// ```
        /// POST: /api/WeatherForecast
        /// {
        ///     "id": "1ac0cf5d-738c-4c9c-9d62-ad07782f8247"
        /// }
        /// ```
        /// </remarks>
        [HttpPost]
        public async Task<bool> Create([FromBody] CreateSampleCommandPayload payload)
        {
            var response = await mediator.Send(new CreateSampleCommand(payload));

            return response;
        }

        private readonly IMediator mediator;
        private readonly ILogger<WeatherForecastController> logger;
    }
}
