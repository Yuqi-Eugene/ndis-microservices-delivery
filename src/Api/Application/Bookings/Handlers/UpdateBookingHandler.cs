using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, Booking>
{
    private readonly AppDbContext _db;

    public UpdateBookingHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(UpdateBookingCommand request, CancellationToken ct)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        var dto = request.Dto;

        entity.ScheduledStartUtc = dto.ScheduledStartUtc;
        entity.DurationMinutes = dto.DurationMinutes;
        entity.ServiceType = dto.ServiceType.Trim();
        entity.Notes = dto.Notes?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}
