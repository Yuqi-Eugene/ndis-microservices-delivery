using Api.Data;
using Api.Dtos.Claims;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClaimsController(AppDbContext db) => _db = db;

    // GET /api/claims
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Claim>>> Get()
    {
        var items = await _db.Claims
            .AsNoTracking()
            .Include(x => x.ServiceDelivery)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return Ok(items);
    }

    // POST /api/claims
    [HttpPost]
    public async Task<ActionResult<Claim>> Create(ClaimCreateDto dto)
    {
        if (dto.ServiceDeliveryId == Guid.Empty)
            return BadRequest(new { message = "ServiceDeliveryId is required." });

        if (dto.Amount <= 0)
            return BadRequest(new { message = "Amount must be positive." });

        var delivery = await _db.ServiceDeliveries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.ServiceDeliveryId);

        if (delivery is null)
            return BadRequest(new { message = "ServiceDelivery not found." });

        if (delivery.Status != "Approved")
            return BadRequest(new { message = "Only Approved deliveries can be claimed." });

        var exists = await _db.Claims
            .AnyAsync(x => x.ServiceDeliveryId == dto.ServiceDeliveryId);

        if (exists)
            return BadRequest(new { message = "Claim already exists for this delivery." });

        var claim = new Claim
        {
            Id = Guid.NewGuid(),
            ServiceDeliveryId = dto.ServiceDeliveryId,
            Amount = dto.Amount,
            Status = "Draft",
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();

        return Ok(claim);
    }
}