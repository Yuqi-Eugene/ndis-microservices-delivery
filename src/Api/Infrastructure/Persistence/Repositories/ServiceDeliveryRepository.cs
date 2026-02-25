using Api.Application.Abstractions.Persistence;
using Api.Data;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence.Repositories;

public class ServiceDeliveryRepository : IServiceDeliveryRepository
{
    private readonly AppDbContext _db;

    public ServiceDeliveryRepository(AppDbContext db) => _db = db;

    public Task<ServiceDelivery?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(ServiceDelivery entity, CancellationToken ct) =>
        await _db.ServiceDeliveries.AddAsync(entity, ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        _db.SaveChangesAsync(ct);
}