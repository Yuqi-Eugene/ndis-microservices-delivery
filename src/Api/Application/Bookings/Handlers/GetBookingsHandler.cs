using Api.Data;
using Api.Dtos;
using Api.Dtos.Bookings;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class GetBookingsHandler : IRequestHandler<GetBookingsQuery, CollectionResponseDto<BookingResponseDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetBookingsHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CollectionResponseDto<BookingResponseDto>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        var query = _db.Bookings
            .AsNoTracking()
            .AsQueryable();

        if (request.ParticipantId.HasValue)
            query = query.Where(x => x.ParticipantId == request.ParticipantId.Value);

        if (request.ProviderId.HasValue)
            query = query.Where(x => x.ProviderId == request.ProviderId.Value);

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc >= request.FromUtc.Value);

        if (request.ToUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc <= request.ToUtc.Value);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync(ct);

        var responseItems = _mapper.Map<List<BookingResponseDto>>(items);
        return new CollectionResponseDto<BookingResponseDto>(responseItems.Count, responseItems);
    }
}
