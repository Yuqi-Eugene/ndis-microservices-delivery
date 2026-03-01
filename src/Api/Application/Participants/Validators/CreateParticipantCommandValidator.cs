using FluentValidation;

namespace Api.Application.Participants;

public sealed class CreateParticipantCommandValidator : AbstractValidator<CreateParticipantCommand>
{
    public CreateParticipantCommandValidator()
    {
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
    }
}
