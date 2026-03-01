using Api.Application.ServiceDeliveries.Commands;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class RejectServiceDeliveryCommandValidator : AbstractValidator<RejectServiceDeliveryCommand>
{
    public RejectServiceDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");
    }
}
