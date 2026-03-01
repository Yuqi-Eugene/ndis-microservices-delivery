using FluentValidation;

namespace Api.Application.Providers;

public sealed class DeleteProviderCommandValidator : AbstractValidator<DeleteProviderCommand>
{
    public DeleteProviderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ProviderId is required.");
    }
}
