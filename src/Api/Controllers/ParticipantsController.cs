using Api.Data;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ParticipantsController(AppDbContext db) => _db = db;

    // GET /api/participants?page=1&pageSize=20&q=gene
    [HttpGet]
    public async Task<ActionResult<object>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Participants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim().ToLower();
            query = query.Where(p =>
                p.FullName.ToLower().Contains(keyword) ||
                (p.Email != null && p.Email.ToLower().Contains(keyword)) ||
                (p.NdisNumber != null && p.NdisNumber.ToLower().Contains(keyword)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    // GET /api/participants/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Participant>> GetById(Guid id)
    {
        var p = await _db.Participants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }

    // POST /api/participants
    [HttpPost]
    public async Task<ActionResult<Participant>> Create(ParticipantCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest(new { message = "FullName is required." });

        var entity = new Participant
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            NdisNumber = dto.NdisNumber?.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim(),
            Status = "Active",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Participants.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    // PUT /api/participants/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Participant>> Update(Guid id, ParticipantUpdateDto dto)
    {
        var entity = await _db.Participants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.FullName))
            return BadRequest(new { message = "FullName is required." });

        entity.FullName = dto.FullName.Trim();
        entity.NdisNumber = dto.NdisNumber?.Trim();
        entity.Phone = dto.Phone?.Trim();
        entity.Email = dto.Email?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE /api/participants/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Participants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();

        _db.Participants.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}