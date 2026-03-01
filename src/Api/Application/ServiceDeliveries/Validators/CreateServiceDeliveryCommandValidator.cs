using Api.Application.ServiceDeliveries.Commands;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class CreateServiceDeliveryCommandValidator : AbstractValidator<CreateServiceDeliveryCommand>
{
    public CreateServiceDeliveryCommandValidator()
    {
        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Unauthorized.");

        RuleFor(x => x.Dto.BookingId)
            .NotEmpty()
            .WithMessage("BookingId is required.");

        RuleFor(x => x.Dto.ActualDurationMinutes)
            .InclusiveBetween(1, 24 * 60)
            .WithMessage("ActualDurationMinutes is invalid.");

        RuleFor(x => x.Dto.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
    }
}
