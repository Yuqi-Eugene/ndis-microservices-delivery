using FluentValidation;

namespace Api.Application.Bookings;

public sealed class GetBookingsQueryValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsQueryValidator()
    {
        RuleFor(x => x.ParticipantId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ParticipantId is invalid.");

        RuleFor(x => x.ProviderId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ProviderId is invalid.");

        RuleFor(x => x.ToUtc)
            .GreaterThanOrEqualTo(x => x.FromUtc!.Value)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue)
            .WithMessage("ToUtc must be greater than or equal to FromUtc.");
    }
}
