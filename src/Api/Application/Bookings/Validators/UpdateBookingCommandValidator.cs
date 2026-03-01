using Api.Domain.Constants;
using FluentValidation;

namespace Api.Application.Bookings;

public sealed class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
        RuleFor(x => x.Dto.ServiceType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.DurationMinutes)
            .InclusiveBetween(1, 24 * 60)
            .WithMessage("DurationMinutes is invalid.");
        RuleFor(x => x.Dto.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
        RuleFor(x => x.Dto.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || BookingStatuses.All.Contains(status.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: Draft, Confirmed, Cancelled.")
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Status));
    }
}
