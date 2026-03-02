using Api.Application.ServiceDeliveries.Queries;
using Api.Data;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class GetServiceDeliveryByIdHandler
    : IRequestHandler<GetServiceDeliveryByIdQuery, ServiceDeliveryResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetServiceDeliveryByIdHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceDeliveryResponseDto> Handle(GetServiceDeliveryByIdQuery request, CancellationToken ct)
    {
        var entity = await _db.ServiceDeliveries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new KeyNotFoundException("ServiceDelivery not found.");

        if (!request.IsAdmin && entity.OwnerUserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("Forbidden.");

        return _mapper.Map<ServiceDeliveryResponseDto>(entity);
    }
}
