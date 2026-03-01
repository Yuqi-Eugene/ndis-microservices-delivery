using Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class DeleteProviderHandler : IRequestHandler<DeleteProviderCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteProviderHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteProviderCommand request, CancellationToken ct)
    {
        var entity = await _db.Providers.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");

        _db.Providers.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
