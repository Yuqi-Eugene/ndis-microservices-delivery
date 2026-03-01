using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class GetBookingsHandler : IRequestHandler<GetBookingsQuery, List<Booking>>
{
    private readonly AppDbContext _db;

    public GetBookingsHandler(AppDbContext db) => _db = db;

    public async Task<List<Booking>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        var query = _db.Bookings
            .AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Provider)
            .AsQueryable();

        if (request.ParticipantId.HasValue)
            query = query.Where(x => x.ParticipantId == request.ParticipantId.Value);

        if (request.ProviderId.HasValue)
            query = query.Where(x => x.ProviderId == request.ProviderId.Value);

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc >= request.FromUtc.Value);

        if (request.ToUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc <= request.ToUtc.Value);

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync(ct);
    }
}
