using FluentValidation;

namespace Api.Application.Participants;

public sealed class GetParticipantsQueryValidator : AbstractValidator<GetParticipantsQuery>
{
    public GetParticipantsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Q)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Q));
    }
}
