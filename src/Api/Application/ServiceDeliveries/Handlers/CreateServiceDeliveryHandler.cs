using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

// This handler contains the business rules for creating a service delivery.
// It is a good example of where state-based domain checks belong.
public sealed class CreateServiceDeliveryHandler
    : IRequestHandler<CreateServiceDeliveryCommand, ServiceDeliveryResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CreateServiceDeliveryHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceDeliveryResponseDto> Handle(CreateServiceDeliveryCommand request, CancellationToken ct)
    {
        // The DTO is the caller's input payload. The handler translates that input into domain work.
        var dto = request.Dto;

        // Load the related booking first because the delivery is not valid on its own.
        // AsNoTracking is used because we only need to read and validate the booking state here.
        var booking = await _db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.BookingId, ct);

        if (booking is null)
            throw new InvalidOperationException("BookingId not found.");

        // Business rule: a provider cannot record actual service delivery time
        // until the original booking has been formally confirmed.
        if (booking.Status != BookingStatuses.Confirmed)
            throw new InvalidOperationException("Booking must be Confirmed before creating a delivery.");

        // Create the new persistence model.
        // IDs and timestamps are generated on the server so clients cannot spoof them.
        var entity = new ServiceDelivery
        {
            Id = Guid.NewGuid(),
            BookingId = dto.BookingId,
            // Ownership matters later for authorization and filtering.
            OwnerUserId = request.CurrentUserId,
            ActualStartUtc = dto.ActualStartUtc,
            ActualDurationMinutes = dto.ActualDurationMinutes,
            // Trim user input before saving so stored text is normalized.
            Notes = dto.Notes?.Trim(),
            // New deliveries always start in Draft; clients cannot choose an arbitrary status at creation time.
            Status = ServiceDeliveryStatuses.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        // Add the entity to the change tracker, then persist it in one database transaction.
        _db.ServiceDeliveries.Add(entity);
        await _db.SaveChangesAsync(ct);

        // Return a DTO rather than the EF entity to keep the API contract explicit.
        return _mapper.Map<ServiceDeliveryResponseDto>(entity);
    }
}
