using Api.Application.ServiceDeliveries.Queries;
using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class GetServiceDeliveryByIdHandler
    : IRequestHandler<GetServiceDeliveryByIdQuery, ServiceDelivery>
{
    private readonly AppDbContext _db;

    public GetServiceDeliveryByIdHandler(AppDbContext db) => _db = db;

    public async Task<ServiceDelivery> Handle(GetServiceDeliveryByIdQuery request, CancellationToken ct)
    {
        var entity = await _db.ServiceDeliveries
            .AsNoTracking()
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");

        if (!request.IsAdmin && entity.OwnerUserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("Forbidden.");

        return entity;
    }
}