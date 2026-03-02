using Api.Dtos;
using Api.Dtos.ServiceDeliveries;
using MediatR;

namespace Api.Application.ServiceDeliveries.Queries;

public sealed record GetServiceDeliveriesQuery(
    Guid? BookingId,
    string? Status,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<CollectionResponseDto<ServiceDeliveryResponseDto>>;
