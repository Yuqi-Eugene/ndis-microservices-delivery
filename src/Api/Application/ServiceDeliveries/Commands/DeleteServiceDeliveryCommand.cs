using MediatR;

namespace Api.Application.ServiceDeliveries.Commands;

public sealed record DeleteServiceDeliveryCommand(
    Guid Id,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<Unit>;