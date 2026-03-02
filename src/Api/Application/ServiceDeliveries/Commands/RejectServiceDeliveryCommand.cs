using MediatR;
using Api.Dtos.ServiceDeliveries;

namespace Api.Application.ServiceDeliveries.Commands;

public sealed record RejectServiceDeliveryCommand(
    Guid Id,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<ServiceDeliveryResponseDto>;
