using Api.Data;
using Api.Dtos.Bookings;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db) => _db = db;

    // GET /api/bookings?participantId=...&providerId=...&fromUtc=...&toUtc=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> Get(
        [FromQuery] Guid? participantId = null,
        [FromQuery] Guid? providerId = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null)
    {
        var query = _db.Bookings
            .AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Provider)
            .AsQueryable();

        if (participantId.HasValue) query = query.Where(x => x.ParticipantId == participantId.Value);
        if (providerId.HasValue) query = query.Where(x => x.ProviderId == providerId.Value);
        if (fromUtc.HasValue) query = query.Where(x => x.ScheduledStartUtc >= fromUtc.Value);
        if (toUtc.HasValue) query = query.Where(x => x.ScheduledStartUtc <= toUtc.Value);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Booking>> GetById(Guid id)
    {
        var entity = await _db.Bookings.AsNoTracking()
            .Include(x => x.Participant)
            .Include(x => x.Provider)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> Create(BookingCreateDto dto)
    {
        if (dto.ParticipantId == Guid.Empty) return BadRequest(new { message = "ParticipantId is required." });
        if (dto.ProviderId == Guid.Empty) return BadRequest(new { message = "ProviderId is required." });
        if (string.IsNullOrWhiteSpace(dto.ServiceType)) return BadRequest(new { message = "ServiceType is required." });
        if (dto.DurationMinutes <= 0 || dto.DurationMinutes > 24 * 60) return BadRequest(new { message = "DurationMinutes is invalid." });

        // Ensure foreign keys exist (important for clear errors)
        var participantExists = await _db.Participants.AsNoTracking().AnyAsync(x => x.Id == dto.ParticipantId);
        if (!participantExists) return BadRequest(new { message = "ParticipantId not found." });

        var providerExists = await _db.Providers.AsNoTracking().AnyAsync(x => x.Id == dto.ProviderId);
        if (!providerExists) return BadRequest(new { message = "ProviderId not found." });

        var entity = new Booking
        {
            Id = Guid.NewGuid(),
            ParticipantId = dto.ParticipantId,
            ProviderId = dto.ProviderId,
            ScheduledStartUtc = dto.ScheduledStartUtc,
            DurationMinutes = dto.DurationMinutes,
            ServiceType = dto.ServiceType.Trim(),
            Notes = dto.Notes?.Trim(),
            Status = "Draft",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Bookings.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Booking>> Update(Guid id, BookingUpdateDto dto)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.ServiceType)) return BadRequest(new { message = "ServiceType is required." });
        if (dto.DurationMinutes <= 0 || dto.DurationMinutes > 24 * 60) return BadRequest(new { message = "DurationMinutes is invalid." });

        entity.ScheduledStartUtc = dto.ScheduledStartUtc;
        entity.DurationMinutes = dto.DurationMinutes;
        entity.ServiceType = dto.ServiceType.Trim();
        entity.Notes = dto.Notes?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // POST /api/bookings/{id}/confirm
    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult<Booking>> Confirm(Guid id)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (entity.Status == "Cancelled") return BadRequest(new { message = "Cancelled booking cannot be confirmed." });

        entity.Status = "Confirmed";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // POST /api/bookings/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<Booking>> Cancel(Guid id)
    {
        var entity = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        entity.Status = "Cancelled";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }
}