using FluentValidation;

namespace Api.Application.Bookings;

public sealed class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
    }
}
