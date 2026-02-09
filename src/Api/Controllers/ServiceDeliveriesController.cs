using Api.Data;
using Api.Dtos.ServiceDeliveries;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceDeliveriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ServiceDeliveriesController(AppDbContext db) => _db = db;

    // GET /api/servicedeliveries?bookingId=...&status=Draft
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDelivery>>> Get(
        [FromQuery] Guid? bookingId = null,
        [FromQuery] string? status = null)
    {
        var query = _db.ServiceDeliveries
            .AsNoTracking()
            .Include(x => x.Booking)
            .AsQueryable();

        if (bookingId.HasValue) query = query.Where(x => x.BookingId == bookingId.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status.Trim());

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceDelivery>> GetById(Guid id)
    {
        var entity = await _db.ServiceDeliveries.AsNoTracking()
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDelivery>> Create(ServiceDeliveryCreateDto dto)
    {
        if (dto.BookingId == Guid.Empty) return BadRequest(new { message = "BookingId is required." });
        if (dto.ActualDurationMinutes <= 0 || dto.ActualDurationMinutes > 24 * 60)
            return BadRequest(new { message = "ActualDurationMinutes is invalid." });

        // Booking must exist
        var booking = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.BookingId);
        if (booking is null) return BadRequest(new { message = "BookingId not found." });

        // Optional business rule: only allow deliveries for confirmed bookings
        if (booking.Status != "Confirmed")
            return BadRequest(new { message = "Booking must be Confirmed before creating a delivery." });

        var entity = new ServiceDelivery
        {
            Id = Guid.NewGuid(),
            BookingId = dto.BookingId,
            ActualStartUtc = dto.ActualStartUtc,
            ActualDurationMinutes = dto.ActualDurationMinutes,
            Notes = dto.Notes?.Trim(),
            Status = "Draft",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.ServiceDeliveries.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ServiceDelivery>> Update(Guid id, ServiceDeliveryUpdateDto dto)
    {
        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (entity.Status != "Draft")
            return BadRequest(new { message = "Only Draft deliveries can be updated." });

        if (dto.ActualDurationMinutes <= 0 || dto.ActualDurationMinutes > 24 * 60)
            return BadRequest(new { message = "ActualDurationMinutes is invalid." });

        entity.ActualStartUtc = dto.ActualStartUtc;
        entity.ActualDurationMinutes = dto.ActualDurationMinutes;
        entity.Notes = dto.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // POST /api/servicedeliveries/{id}/submit
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<ServiceDelivery>> Submit(Guid id)
    {
        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (entity.Status != "Draft")
            return BadRequest(new { message = "Only Draft deliveries can be submitted." });

        entity.Status = "Submitted";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (entity.Status != "Draft")
            return BadRequest(new { message = "Only Draft deliveries can be deleted." });

        _db.ServiceDeliveries.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // POST /api/servicedeliveries/{id}/approve
    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<ServiceDelivery>> Approve(Guid id)
    {
        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (entity.Status != "Submitted")
            return BadRequest(new { message = "Only Submitted deliveries can be approved." });

        entity.Status = "Approved";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // POST /api/servicedeliveries/{id}/reject
    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<ServiceDelivery>> Reject(Guid id)
    {
        var entity = await _db.ServiceDeliveries.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (entity.Status != "Submitted")
            return BadRequest(new { message = "Only Submitted deliveries can be rejected." });

        entity.Status = "Rejected";
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

}