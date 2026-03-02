using Api.Data;
using Api.Dtos.Bookings;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, BookingResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetBookingByIdHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        var entity = await _db.Bookings.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        return _mapper.Map<BookingResponseDto>(entity);
    }
}
