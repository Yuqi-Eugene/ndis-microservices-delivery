using FluentValidation;

namespace Api.Application.Participants;

public sealed class GetParticipantByIdQueryValidator : AbstractValidator<GetParticipantByIdQuery>
{
    public GetParticipantByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ParticipantId is required.");
    }
}
