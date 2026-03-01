using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Claims;

public sealed class CreateClaimHandler : IRequestHandler<CreateClaimCommand, Claim>
{
    private readonly AppDbContext _db;

    public CreateClaimHandler(AppDbContext db) => _db = db;

    public async Task<Claim> Handle(CreateClaimCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var delivery = await _db.ServiceDeliveries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.ServiceDeliveryId, ct);

        if (delivery is null)
            throw new InvalidOperationException("ServiceDelivery not found.");

        if (delivery.Status != ServiceDeliveryStatuses.Approved)
            throw new InvalidOperationException("Only Approved deliveries can be claimed.");

        var exists = await _db.Claims
            .AnyAsync(x => x.ServiceDeliveryId == dto.ServiceDeliveryId, ct);

        if (exists)
            throw new InvalidOperationException("Claim already exists for this delivery.");

        var claim = new Claim
        {
            Id = Guid.NewGuid(),
            ServiceDeliveryId = dto.ServiceDeliveryId,
            Amount = dto.Amount,
            Status = ClaimStatuses.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync(ct);

        return claim;
    }
}
