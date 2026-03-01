using Api.Data;
using Api.Domain.Constants;
using Api.Domain.Entities;
using Api.Dtos.Bookings;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Bookings;

public sealed record GetBookingsQuery(
    Guid? ParticipantId = null,
    Guid? ProviderId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<List<Booking>>;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<Booking>;
public sealed record CreateBookingCommand(BookingCreateDto Dto) : IRequest<Booking>;
public sealed record UpdateBookingCommand(Guid Id, BookingUpdateDto Dto) : IRequest<Booking>;
public sealed record ConfirmBookingCommand(Guid Id) : IRequest<Booking>;
public sealed record CancelBookingCommand(Guid Id) : IRequest<Booking>;

public sealed class GetBookingsHandler : IRequestHandler<GetBookingsQuery, List<Booking>>
{
    private readonly AppDbContext _db;

    public GetBookingsHandler(AppDbContext db) => _db = db;

    public async Task<List<Booking>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        var query = _db.Bookings
            .AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Provider)
            .AsQueryable();

        if (request.ParticipantId.HasValue)
            query = query.Where(x => x.ParticipantId == request.ParticipantId.Value);

        if (request.ProviderId.HasValue)
            query = query.Where(x => x.ProviderId == request.ProviderId.Value);

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc >= request.FromUtc.Value);

        if (request.ToUtc.HasValue)
            query = query.Where(x => x.ScheduledStartUtc <= request.ToUtc.Value);

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync(ct);
    }
}

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

public sealed class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Booking>
{
    private readonly AppDbContext _db;

    public CreateBookingHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(CreateBookingCommand request, CancellationToken ct)
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

        return entity;
    }
}

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

public sealed class ConfirmBookingHandler : IRequestHandler<ConfirmBookingCommand, Booking>
{
    private readonly AppDbContext _db;

    public ConfirmBookingHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(ConfirmBookingCommand request, CancellationToken ct)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        if (entity.Status == BookingStatuses.Cancelled)
            throw new InvalidOperationException("Cancelled booking cannot be confirmed.");

        entity.Status = BookingStatuses.Confirmed;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}

public sealed class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Booking>
{
    private readonly AppDbContext _db;

    public CancelBookingHandler(AppDbContext db) => _db = db;

    public async Task<Booking> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        entity.Status = BookingStatuses.Cancelled;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}

public sealed class GetBookingsQueryValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsQueryValidator()
    {
        RuleFor(x => x.ParticipantId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ParticipantId is invalid.");

        RuleFor(x => x.ProviderId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ProviderId is invalid.");

        RuleFor(x => x.ToUtc)
            .GreaterThanOrEqualTo(x => x.FromUtc!.Value)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue)
            .WithMessage("ToUtc must be greater than or equal to FromUtc.");
    }
}

public sealed class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
    }
}

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.Dto.ParticipantId).NotEmpty().WithMessage("ParticipantId is required.");
        RuleFor(x => x.Dto.ProviderId).NotEmpty().WithMessage("ProviderId is required.");
        RuleFor(x => x.Dto.ServiceType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.DurationMinutes)
            .InclusiveBetween(1, 24 * 60)
            .WithMessage("DurationMinutes is invalid.");
        RuleFor(x => x.Dto.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
    }
}

public sealed class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
        RuleFor(x => x.Dto.ServiceType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.DurationMinutes)
            .InclusiveBetween(1, 24 * 60)
            .WithMessage("DurationMinutes is invalid.");
        RuleFor(x => x.Dto.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Notes));
        RuleFor(x => x.Dto.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || BookingStatuses.All.Contains(status.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: Draft, Confirmed, Cancelled.")
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Status));
    }
}

public sealed class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
    }
}

public sealed class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("BookingId is required.");
    }
}
