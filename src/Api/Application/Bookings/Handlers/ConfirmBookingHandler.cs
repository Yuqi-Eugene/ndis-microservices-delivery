using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand, Booking>
{
    private readonly AppDbContext _db;

    public ConfirmBookingHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(ConfirmBookingCommand request, CancellationToken ct)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        if (entity.Status == BookingStatuses.Cancelled)
            throw new InvalidOperationException("Cancelled booking cannot be confirmed.");

        entity.Status = BookingStatuses.Confirmed;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}
