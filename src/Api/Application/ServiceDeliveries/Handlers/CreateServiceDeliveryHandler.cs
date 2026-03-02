using Api.Application.ServiceDeliveries.Commands;
using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.ServiceDeliveries.Handlers;

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
        var dto = request.Dto;

        var booking = await _db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.BookingId, ct);

        if (booking is null)
            throw new InvalidOperationException("BookingId not found.");

        if (booking.Status != BookingStatuses.Confirmed)
            throw new InvalidOperationException("Booking must be Confirmed before creating a delivery.");

        var entity = new ServiceDelivery
        {
            Id = Guid.NewGuid(),
            BookingId = dto.BookingId,
            OwnerUserId = request.CurrentUserId,
            ActualStartUtc = dto.ActualStartUtc,
            ActualDurationMinutes = dto.ActualDurationMinutes,
            Notes = dto.Notes?.Trim(),
            Status = ServiceDeliveryStatuses.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.ServiceDeliveries.Add(entity);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<ServiceDeliveryResponseDto>(entity);
    }
}
