using Api.Application.Abstractions.Persistence;
using Api.Application.ServiceDeliveries.Commands;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class RejectServiceDeliveryHandler
    : IRequestHandler<RejectServiceDeliveryCommand, ServiceDelivery>
{
    private readonly IServiceDeliveryRepository _repo;

    public RejectServiceDeliveryHandler(IServiceDeliveryRepository repo) => _repo = repo;

    public async Task<ServiceDelivery> Handle(RejectServiceDeliveryCommand request, CancellationToken ct)
    {
        if (!request.IsAdmin)
            throw new UnauthorizedAccessException("Admin role required.");

        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");

        if (entity.Status != "Submitted")
            throw new InvalidOperationException("Only Submitted deliveries can be rejcted.");

        entity.Status = "Rejected";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        return entity;
    }
}