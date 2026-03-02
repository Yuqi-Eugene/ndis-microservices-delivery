using Api.Dtos.ServiceDeliveries;
using MediatR;

namespace Api.Application.ServiceDeliveries.Queries;

public sealed record GetServiceDeliveryByIdQuery(
    Guid Id,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<ServiceDeliveryResponseDto>;
