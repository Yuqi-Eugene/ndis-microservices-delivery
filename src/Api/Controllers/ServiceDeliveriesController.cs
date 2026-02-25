using Api.Dtos.ServiceDeliveries;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using Api.Application.ServiceDeliveries.Commands;
using Api.Application.ServiceDeliveries.Queries;


namespace Api.Controllers;


[ApiController]
[Route("api/[controller]")]



public class ServiceDeliveriesController : ControllerBase
{
    // Get the current user ID and check if the user is an admin
    private string CurrentUserId =>
    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    private bool IsAdmin =>
    User.IsInRole("Admin");

    private readonly IMediator _mediator;

    public ServiceDeliveriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDelivery>>> Get(
        [FromQuery] Guid? bookingId = null,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _mediator.Send(
                new GetServiceDeliveriesQuery(
                    bookingId,
                    status,
                    CurrentUserId,
                    IsAdmin),
                ct);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceDelivery>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(
                new GetServiceDeliveryByIdQuery(
                    id,
                    CurrentUserId,
                    IsAdmin),
                ct);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpPost]
    public async Task<ActionResult<ServiceDelivery>> Create(ServiceDeliveryCreateDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(
                new CreateServiceDeliveryCommand(
                    dto,
                    CurrentUserId,
                    IsAdmin),
                ct);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ServiceDelivery>> Update(Guid id, ServiceDeliveryUpdateDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(
                new UpdateServiceDeliveryCommand(
                    id,
                    dto,
                    CurrentUserId,
                    IsAdmin),
                ct);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpPost("{id:guid}/submit")]

    public async Task<ActionResult> Submit(Guid id, CancellationToken ct)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var result = await _mediator.Send(new SubmitServiceDeliveryCommand(id, currentUserId, isAdmin), ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(
                new DeleteServiceDeliveryCommand(
                    id,
                    CurrentUserId,
                    IsAdmin),
                ct);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/servicedeliveries/{id}/approve
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/approve")]

    public async Task<ActionResult> Approve(Guid id, CancellationToken ct)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var result = await _mediator.Send(new ApproveServiceDeliveryCommand(id, currentUserId, isAdmin), ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/servicedeliveries/{id}/reject
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/reject")]

    public async Task<ActionResult> Reject(Guid id, CancellationToken ct)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var result = await _mediator.Send(new RejectServiceDeliveryCommand(id, currentUserId, isAdmin), ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}