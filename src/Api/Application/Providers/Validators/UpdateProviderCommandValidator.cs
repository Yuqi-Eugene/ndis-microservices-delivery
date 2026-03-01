using FluentValidation;

namespace Api.Application.Providers;

public sealed class UpdateProviderCommandValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ProviderId is required.");
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Abn)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Abn));
        RuleFor(x => x.Dto.ContactPhone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactPhone));
        RuleFor(x => x.Dto.ContactEmail)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactEmail));
        RuleFor(x => x.Dto.Status)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Status));
    }
}
