using MediatR;
using Api.Domain.Entities;

namespace Api.Application.ServiceDeliveries.Commands;

public sealed record ApproveServiceDeliveryCommand(
    Guid Id,
    string CurrentUserId,
    bool IsAdmin
) : IRequest<ServiceDelivery>;