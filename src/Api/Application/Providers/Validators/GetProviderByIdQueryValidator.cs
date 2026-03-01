using FluentValidation;

namespace Api.Application.Providers;

public sealed class GetProviderByIdQueryValidator : AbstractValidator<GetProviderByIdQuery>
{
    public GetProviderByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ProviderId is required.");
    }
}
