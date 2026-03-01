using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Booking>
{
    private readonly AppDbContext _db;

    public CancelBookingHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        entity.Status = BookingStatuses.Cancelled;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}
