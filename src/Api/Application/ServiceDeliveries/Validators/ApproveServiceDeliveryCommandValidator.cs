using Api.Application.ServiceDeliveries.Commands;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class ApproveServiceDeliveryCommandValidator : AbstractValidator<ApproveServiceDeliveryCommand>
{
    public ApproveServiceDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");
    }
}
