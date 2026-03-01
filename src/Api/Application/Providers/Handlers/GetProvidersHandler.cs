using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class GetProvidersHandler : IRequestHandler<GetProvidersQuery, List<Provider>>
{
    private readonly AppDbContext _db;

    public GetProvidersHandler(AppDbContext db) => _db = db;

    public Task<List<Provider>> Handle(GetProvidersQuery request, CancellationToken ct)
        => _db.Providers.AsNoTracking().ToListAsync(ct);
}
