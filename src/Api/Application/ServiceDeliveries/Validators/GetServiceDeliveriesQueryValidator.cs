using Api.Application.ServiceDeliveries.Queries;
using Api.Domain.Constants;
using FluentValidation;

namespace Api.Application.ServiceDeliveries.Validators;

public sealed class GetServiceDeliveriesQueryValidator : AbstractValidator<GetServiceDeliveriesQuery>
{
    public GetServiceDeliveriesQueryValidator()
    {
        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .When(x => !x.IsAdmin)
            .WithMessage("Unauthorized.");

        RuleFor(x => x.BookingId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("BookingId is invalid.");

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || ServiceDeliveryStatuses.All.Contains(status.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: Draft, Submitted, Approved, Rejected.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}
