using Api.Application.ServiceDeliveries.Queries;
using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class GetServiceDeliveriesHandler
    : IRequestHandler<GetServiceDeliveriesQuery, List<ServiceDelivery>>
{
    private readonly AppDbContext _db;

    public GetServiceDeliveriesHandler(AppDbContext db) => _db = db;

    public async Task<List<ServiceDelivery>> Handle(GetServiceDeliveriesQuery request, CancellationToken ct)
    {
        var query = _db.ServiceDeliveries
            .AsNoTracking()
            .Include(x => x.Booking)
            .AsQueryable();

        if (!request.IsAdmin)
            query = query.Where(x => x.OwnerUserId == request.CurrentUserId);

        if (request.BookingId.HasValue)
            query = query.Where(x => x.BookingId == request.BookingId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.Status == request.Status.Trim());

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync(ct);
    }
}