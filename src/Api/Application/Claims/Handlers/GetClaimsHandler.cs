using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Claims;

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
