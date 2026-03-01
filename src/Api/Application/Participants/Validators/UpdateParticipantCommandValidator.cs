using FluentValidation;

namespace Api.Application.Participants;

public sealed class UpdateParticipantCommandValidator : AbstractValidator<UpdateParticipantCommand>
{
    public UpdateParticipantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ParticipantId is required.");
        RuleFor(x => x.Dto.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.NdisNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.NdisNumber));
        RuleFor(x => x.Dto.Phone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Phone));
        RuleFor(x => x.Dto.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));
        RuleFor(x => x.Dto.Status)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Status));
    }
}
