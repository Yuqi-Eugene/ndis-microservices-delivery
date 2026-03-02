using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using Api.Domain.Constants;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class ApproveServiceDeliveryHandler
    : IRequestHandler<ApproveServiceDeliveryCommand, ServiceDeliveryResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ApproveServiceDeliveryHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceDeliveryResponseDto> Handle(ApproveServiceDeliveryCommand request, CancellationToken ct)
    {
        if (!request.IsAdmin)
            throw new UnauthorizedAccessException("Admin role required.");

        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");

        if (entity.Status != ServiceDeliveryStatuses.Submitted)
            throw new InvalidOperationException("Only Submitted deliveries can be approved.");

        entity.Status = ServiceDeliveryStatuses.Approved;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ServiceDeliveryResponseDto>(entity);
    }
}
