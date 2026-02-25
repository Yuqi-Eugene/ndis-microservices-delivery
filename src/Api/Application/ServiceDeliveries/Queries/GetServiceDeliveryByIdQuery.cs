using Api.Domain.Entities;
using MediatR;

namespace Api.Application.ServiceDeliveries.Queries;

public sealed record GetServiceDeliveryByIdQuery(
    Guid Id,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<ServiceDelivery>;