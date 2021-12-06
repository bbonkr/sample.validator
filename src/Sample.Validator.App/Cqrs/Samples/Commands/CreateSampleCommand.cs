using System;

using FluentValidation;

using MediatR;

namespace Sample.Validator.App.Cqrs.Samples.Commands
{
    public class CreateSampleCommand : IRequest<bool>
    {
        public CreateSampleCommand(CreateSampleCommandPayload payload)
        {
            Payload = payload;
        }

        public CreateSampleCommandPayload Payload { get; }
    }

    public class CreateSampleCommandPayload
    {
        public Guid? Id { get; set; }
    }

    public class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
    {
        public CreateSampleCommandValidator()
        {
            RuleFor(x => x.Payload).NotNull().WithMessage("Payload is required.");

            RuleFor(x => x.Payload).SetValidator(new CreateSampleCommandPayloadValidator());
        }
    }

    public class CreateSampleCommandPayloadValidator :AbstractValidator<CreateSampleCommandPayload>
    {
        public CreateSampleCommandPayloadValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("Id is required.");
        }
    }
}
