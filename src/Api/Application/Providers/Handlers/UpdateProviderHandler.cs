using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class UpdateProviderHandler : IRequestHandler<UpdateProviderCommand, Provider>
{
    private readonly AppDbContext _db;

    public UpdateProviderHandler(AppDbContext db) => _db = db;

    public async Task<Provider> Handle(UpdateProviderCommand request, CancellationToken ct)
    {
        var entity = await _db.Providers.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");

        var dto = request.Dto;

        entity.Name = dto.Name.Trim();
        entity.Abn = dto.Abn?.Trim();
        entity.ContactEmail = dto.ContactEmail?.Trim();
        entity.ContactPhone = dto.ContactPhone?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}
