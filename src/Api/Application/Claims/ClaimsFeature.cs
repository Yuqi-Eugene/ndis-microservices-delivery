using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using Api.Dtos.Claims;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Claims;

public sealed record GetClaimsQuery() : IRequest<List<Claim>>;
public sealed record CreateClaimCommand(ClaimCreateDto Dto) : IRequest<Claim>;

public sealed class GetClaimsHandler : IRequestHandler<GetClaimsQuery, List<Claim>>
{
    private readonly AppDbContext _db;

    public GetClaimsHandler(AppDbContext db) => _db = db;

    public Task<List<Claim>> Handle(GetClaimsQuery request, CancellationToken ct)
        => _db.Claims
            .AsNoTracking()
            .Include(x => x.ServiceDelivery)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
}

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

public sealed class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.Dto.ServiceDeliveryId)
            .NotEmpty()
            .WithMessage("ServiceDeliveryId is required.");
        RuleFor(x => x.Dto.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be positive.");
    }
}
