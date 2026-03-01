using Api.Application.ServiceDeliveries.Queries;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class GetServiceDeliveryByIdQueryValidator : AbstractValidator<GetServiceDeliveryByIdQuery>
{
    public GetServiceDeliveryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .When(x => !x.IsAdmin)
            .WithMessage("Unauthorized.");
    }
}
