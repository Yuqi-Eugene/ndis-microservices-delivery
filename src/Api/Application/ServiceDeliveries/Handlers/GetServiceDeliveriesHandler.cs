using Api.Application.ServiceDeliveries.Queries;
using Api.Data;
using Api.Dtos;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class GetServiceDeliveriesHandler
    : IRequestHandler<GetServiceDeliveriesQuery, CollectionResponseDto<ServiceDeliveryResponseDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetServiceDeliveriesHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CollectionResponseDto<ServiceDeliveryResponseDto>> Handle(GetServiceDeliveriesQuery request, CancellationToken ct)
    {
        var query = _db.ServiceDeliveries
            .AsNoTracking()
            .AsQueryable();

        if (!request.IsAdmin)
            query = query.Where(x => x.OwnerUserId == request.CurrentUserId);

        if (request.BookingId.HasValue)
            query = query.Where(x => x.BookingId == request.BookingId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.Status == request.Status.Trim());

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync(ct);

        var responseItems = _mapper.Map<List<ServiceDeliveryResponseDto>>(items);
        return new CollectionResponseDto<ServiceDeliveryResponseDto>(responseItems.Count, responseItems);
    }
}
