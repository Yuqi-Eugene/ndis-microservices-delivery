using FluentValidation;

namespace Api.Application.Providers;

public sealed class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderCommandValidator()
    {
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
    }
}
