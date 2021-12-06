
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Sample.Validator.App.Cqrs.Samples.Commands
{
    public class CreateSampleCommandHandler : IRequestHandler<CreateSampleCommand, bool>
    {
        public CreateSampleCommandHandler(ILogger<CreateSampleCommandHandler> logger)
        {
            this.logger = logger;
        }

        public Task<bool> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("CreateSampleCommand executed!");

            return Task.FromResult(true);
        }

        private readonly ILogger logger;
    }
}
