using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class DeleteServiceDeliveryHandler
    : IRequestHandler<DeleteServiceDeliveryCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteServiceDeliveryHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteServiceDeliveryCommand request, CancellationToken ct)
    {
        var entity = await _db.ServiceDeliveries
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");

        if (!request.IsAdmin && entity.OwnerUserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("Forbidden.");

        if (entity.Status != "Draft")
            throw new InvalidOperationException("Only Draft deliveries can be deleted.");

        _db.ServiceDeliveries.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}