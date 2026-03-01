using FluentValidation;

namespace Api.Application.Bookings;

public sealed class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
    }
}
