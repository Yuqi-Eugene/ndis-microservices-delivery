using Api.Application.Abstractions.Persistence;
using Api.Application.ServiceDeliveries.Commands;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class SubmitServiceDeliveryHandler
    : IRequestHandler<SubmitServiceDeliveryCommand, ServiceDelivery>
{
    private readonly IServiceDeliveryRepository _repo;

    public SubmitServiceDeliveryHandler(IServiceDeliveryRepository repo) => _repo = repo;

    public async Task<ServiceDelivery> Handle(SubmitServiceDeliveryCommand request, CancellationToken ct)
    {

        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");
            
        if (!request.IsAdmin && entity.OwnerUserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("You can only submit your own service deliveries.");        

        if (entity.Status != "Draft")
            throw new InvalidOperationException("Only Draft deliveries can be submitted.");

        entity.Status = "Submitted";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        return entity;
    }
}