using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, Booking>
{
    private readonly AppDbContext _db;

    public GetBookingByIdHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(GetBookingByIdQuery request, CancellationToken ct)
        => await _db.Bookings.AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Provider)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");
}
