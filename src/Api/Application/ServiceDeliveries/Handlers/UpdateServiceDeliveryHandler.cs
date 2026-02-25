using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class UpdateServiceDeliveryHandler
    : IRequestHandler<UpdateServiceDeliveryCommand, ServiceDelivery>
{
    private readonly AppDbContext _db;

    public UpdateServiceDeliveryHandler(AppDbContext db) => _db = db;

    public async Task<ServiceDelivery> Handle(UpdateServiceDeliveryCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var entity = await _db.ServiceDeliveries
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");

        if (!request.IsAdmin && entity.OwnerUserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("Forbidden.");

        if (entity.Status != "Draft")
            throw new InvalidOperationException("Only Draft deliveries can be updated.");

        if (dto.ActualDurationMinutes <= 0 || dto.ActualDurationMinutes > 24 * 60)
            throw new InvalidOperationException("ActualDurationMinutes is invalid.");

        entity.ActualStartUtc = dto.ActualStartUtc;
        entity.ActualDurationMinutes = dto.ActualDurationMinutes;
        entity.Notes = dto.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}