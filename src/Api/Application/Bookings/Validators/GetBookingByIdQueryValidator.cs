using FluentValidation;

namespace Api.Application.Bookings;

public sealed class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
    }
}
