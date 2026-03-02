using Api.Dtos.ServiceDeliveries;
using MediatR;

namespace Api.Application.ServiceDeliveries.Commands;

public sealed record UpdateServiceDeliveryCommand(
    Guid Id,
    ServiceDeliveryUpdateDto Dto,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<ServiceDeliveryResponseDto>;
