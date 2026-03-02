using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using Api.Dtos.Claims;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Claims;

// This handler enforces the rules for turning an approved service delivery into a claim.
public sealed class CreateClaimHandler : IRequestHandler<CreateClaimCommand, ClaimResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CreateClaimHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ClaimResponseDto> Handle(CreateClaimCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        // The referenced service delivery must exist before a claim can be attached to it.
        var delivery = await _db.ServiceDeliveries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.ServiceDeliveryId, ct);

        if (delivery is null)
            throw new InvalidOperationException("ServiceDelivery not found.");

        // Business rule: only approved work can be claimed.
        if (delivery.Status != ServiceDeliveryStatuses.Approved)
            throw new InvalidOperationException("Only Approved deliveries can be claimed.");

        // Guard against duplicate claims at the application layer.
        // The database also reinforces this with a unique index.
        var exists = await _db.Claims
            .AnyAsync(x => x.ServiceDeliveryId == dto.ServiceDeliveryId, ct);

        if (exists)
            throw new InvalidOperationException("Claim already exists for this delivery.");

        // Create the new claim in its initial workflow state.
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

        return _mapper.Map<ClaimResponseDto>(claim);
    }
}
