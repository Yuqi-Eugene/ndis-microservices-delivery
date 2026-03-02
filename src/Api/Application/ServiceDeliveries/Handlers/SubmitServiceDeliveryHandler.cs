using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using Api.Domain.Constants;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class SubmitServiceDeliveryHandler
    : IRequestHandler<SubmitServiceDeliveryCommand, ServiceDeliveryResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public SubmitServiceDeliveryHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceDeliveryResponseDto> Handle(SubmitServiceDeliveryCommand request, CancellationToken ct)
    {

        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");
            
        if (!request.IsAdmin && entity.OwnerUserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("You can only submit your own service deliveries.");        

        if (entity.Status != ServiceDeliveryStatuses.Draft)
            throw new InvalidOperationException("Only Draft deliveries can be submitted.");

        entity.Status = ServiceDeliveryStatuses.Submitted;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ServiceDeliveryResponseDto>(entity);
    }
}
