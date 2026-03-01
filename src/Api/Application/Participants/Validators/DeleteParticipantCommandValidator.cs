using FluentValidation;

namespace Api.Application.Participants;

public sealed class DeleteParticipantCommandValidator : AbstractValidator<DeleteParticipantCommand>
{
    public DeleteParticipantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ParticipantId is required.");
    }
}
