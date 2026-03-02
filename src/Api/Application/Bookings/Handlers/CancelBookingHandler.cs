using Api.Data;
using Api.Domain.Constants;
using Api.Dtos.Bookings;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class CancelBookingHandler : IRequestHandler<CancelBookingCommand, BookingResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CancelBookingHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        entity.Status = BookingStatuses.Cancelled;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<BookingResponseDto>(entity);
    }
}
