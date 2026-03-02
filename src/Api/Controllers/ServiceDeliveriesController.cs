using Api.Application.ServiceDeliveries.Commands;
using Api.Application.ServiceDeliveries.Queries;
using Api.Dtos;
using Api.Dtos.ServiceDeliveries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceDeliveriesController : ControllerBase
{
    private readonly IMediator _mediator;

    // These helpers read the authenticated user from HttpContext.User.
    // The JWT middleware populates this principal after token validation succeeds.
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
    private bool IsAdmin => User.IsInRole("Admin");

    public ServiceDeliveriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpGet]
    public async Task<ActionResult<CollectionResponseDto<ServiceDeliveryResponseDto>>> Get(
        [FromQuery] Guid? bookingId = null,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        // Controllers should stay thin: collect HTTP input, forward to a request object, return HTTP output.
        var result = await _mediator.Send(
            new GetServiceDeliveriesQuery(
                bookingId,
                status,
                CurrentUserId,
                IsAdmin),
            ct);

        return Ok(result);
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceDeliveryResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetServiceDeliveryByIdQuery(
                id,
                CurrentUserId,
                IsAdmin),
            ct);

        return Ok(result);
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpPost]
    public async Task<ActionResult<ServiceDeliveryResponseDto>> Create(ServiceDeliveryCreateDto dto, CancellationToken ct)
    {
        // Passing user context into the command allows the handler to enforce ownership rules.
        var result = await _mediator.Send(
            new CreateServiceDeliveryCommand(
                dto,
                CurrentUserId,
                IsAdmin),
            ct);

        // 201 Created is the correct REST response when a new resource is created successfully.
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ServiceDeliveryResponseDto>> Update(Guid id, ServiceDeliveryUpdateDto dto, CancellationToken ct)
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

    [Authorize(Roles = "Provider,Admin")]
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult> Submit(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SubmitServiceDeliveryCommand(
                id,
                CurrentUserId,
                IsAdmin),
            ct);

        return Ok(result);
    }

    [Authorize(Roles = "Provider,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(
            new DeleteServiceDeliveryCommand(
                id,
                CurrentUserId,
                IsAdmin),
            ct);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult> Approve(Guid id, CancellationToken ct)
    {
        // Approval is admin-only because it represents a privileged workflow decision.
        var result = await _mediator.Send(
            new ApproveServiceDeliveryCommand(
                id,
                CurrentUserId,
                IsAdmin),
            ct);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult> Reject(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RejectServiceDeliveryCommand(
                id,
                CurrentUserId,
                IsAdmin),
            ct);

        return Ok(result);
    }
}
