using Api.Application.ServiceDeliveries.Commands;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class UpdateServiceDeliveryCommandValidator : AbstractValidator<UpdateServiceDeliveryCommand>
{
    public UpdateServiceDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .When(x => !x.IsAdmin)
            .WithMessage("Unauthorized.");

        RuleFor(x => x.Dto.ActualDurationMinutes)
            .InclusiveBetween(1, 24 * 60)
            .WithMessage("ActualDurationMinutes is invalid.");

        RuleFor(x => x.Dto.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
    }
}
