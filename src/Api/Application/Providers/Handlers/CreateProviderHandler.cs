using Api.Data;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Providers;

public sealed class CreateProviderHandler : IRequestHandler<CreateProviderCommand, Provider>
{
    private readonly AppDbContext _db;

    public CreateProviderHandler(AppDbContext db) => _db = db;

    public async Task<Provider> Handle(CreateProviderCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var entity = new Provider
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Abn = dto.Abn?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            ContactPhone = dto.ContactPhone?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Providers.Add(entity);
        await _db.SaveChangesAsync(ct);

        return entity;
    }
}
