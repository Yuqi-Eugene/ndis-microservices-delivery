using FluentValidation;

namespace Api.Application.Bookings;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.Dto.ParticipantId).NotEmpty().WithMessage("ParticipantId is required.");
        RuleFor(x => x.Dto.ProviderId).NotEmpty().WithMessage("ProviderId is required.");
        RuleFor(x => x.Dto.ServiceType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.DurationMinutes)
            .InclusiveBetween(1, 24 * 60)
            .WithMessage("DurationMinutes is invalid.");
        RuleFor(x => x.Dto.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
    }
}
