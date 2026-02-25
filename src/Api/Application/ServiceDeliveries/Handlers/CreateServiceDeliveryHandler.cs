using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

public sealed class CreateServiceDeliveryHandler
    : IRequestHandler<CreateServiceDeliveryCommand, ServiceDelivery>
{
    private readonly AppDbContext _db;

    public CreateServiceDeliveryHandler(AppDbContext db) => _db = db;

    public async Task<ServiceDelivery> Handle(CreateServiceDeliveryCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        if (string.IsNullOrWhiteSpace(request.CurrentUserId))
            throw new UnauthorizedAccessException("Unauthorized.");

        if (dto.BookingId == Guid.Empty)
            throw new InvalidOperationException("BookingId is required.");

        if (dto.ActualDurationMinutes <= 0 || dto.ActualDurationMinutes > 24 * 60)
            throw new InvalidOperationException("ActualDurationMinutes is invalid.");

        var booking = await _db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.BookingId, ct);

        if (booking is null)
            throw new InvalidOperationException("BookingId not found.");

        if (booking.Status != "Confirmed")
            throw new InvalidOperationException("Booking must be Confirmed before creating a delivery.");

        var entity = new ServiceDelivery
        {
            Id = Guid.NewGuid(),
            BookingId = dto.BookingId,
            OwnerUserId = request.CurrentUserId,
            ActualStartUtc = dto.ActualStartUtc,
            ActualDurationMinutes = dto.ActualDurationMinutes,
            Notes = dto.Notes?.Trim(),
            Status = "Draft",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.ServiceDeliveries.Add(entity);
        await _db.SaveChangesAsync(ct);

        return entity;
    }
}