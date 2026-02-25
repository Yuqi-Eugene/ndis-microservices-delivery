using Api.Domain.Entities;

namespace Api.Application.Abstractions.Persistence;


// this is the port that application depends on. on EF here.
public interface IServiceDeliveryRepository
{
    Task<ServiceDelivery?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(ServiceDelivery entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}