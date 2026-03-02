using Api.Dtos.ServiceDeliveries;
using MediatR;

namespace Api.Application.ServiceDeliveries.Commands;

public sealed record CreateServiceDeliveryCommand(
    ServiceDeliveryCreateDto Dto,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<ServiceDeliveryResponseDto>;
