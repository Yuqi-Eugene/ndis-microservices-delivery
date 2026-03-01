using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class GetProviderByIdHandler : IRequestHandler<GetProviderByIdQuery, Provider>
{
    private readonly AppDbContext _db;

    public GetProviderByIdHandler(AppDbContext db) => _db = db;

    public async Task<Provider> Handle(GetProviderByIdQuery request, CancellationToken ct)
        => await _db.Providers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");
}
