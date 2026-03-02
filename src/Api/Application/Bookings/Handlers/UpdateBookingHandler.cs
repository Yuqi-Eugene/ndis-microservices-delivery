using Api.Data;
using Api.Dtos.Bookings;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, BookingResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UpdateBookingHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> Handle(UpdateBookingCommand request, CancellationToken ct)
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
        return _mapper.Map<BookingResponseDto>(entity);
    }
}
