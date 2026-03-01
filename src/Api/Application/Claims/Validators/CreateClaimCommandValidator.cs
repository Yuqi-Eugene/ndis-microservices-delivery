using FluentValidation;

namespace Api.Application.Claims;

public sealed class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.Dto.ServiceDeliveryId)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");
        RuleFor(x => x.Dto.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be positive.");
    }
}
