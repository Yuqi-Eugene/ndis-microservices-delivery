using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using Api.Dtos.Bookings;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CreateBookingHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var participantExists = await _db.Participants.AsNoTracking().AnyAsync(x => x.Id == dto.ParticipantId, ct);
        if (!participantExists)
            throw new InvalidOperationException("ParticipantId not found.");

        var providerExists = await _db.Providers.AsNoTracking().AnyAsync(x => x.Id == dto.ProviderId, ct);
        if (!providerExists)
            throw new InvalidOperationException("ProviderId not found.");

        var entity = new Booking
        {
            Id = Guid.NewGuid(),
            ParticipantId = dto.ParticipantId,
            ProviderId = dto.ProviderId,
            ScheduledStartUtc = dto.ScheduledStartUtc,
            DurationMinutes = dto.DurationMinutes,
            ServiceType = dto.ServiceType.Trim(),
            Notes = dto.Notes?.Trim(),
            Status = BookingStatuses.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Bookings.Add(entity);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<BookingResponseDto>(entity);
    }
}
