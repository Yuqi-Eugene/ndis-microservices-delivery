using Api.Application.ServiceDeliveries.Commands;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class SubmitServiceDeliveryCommandValidator : AbstractValidator<SubmitServiceDeliveryCommand>
{
    public SubmitServiceDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .When(x => !x.IsAdmin)
            .WithMessage("Unauthorized.");
    }
}
