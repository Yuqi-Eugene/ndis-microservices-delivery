using Api.Application.Bookings;
using Api.Dtos;
using Api.Dtos.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator) => _mediator = mediator;

    // GET /api/bookings?participantId=...&providerId=...&fromUtc=...&toUtc=...
    [HttpGet]
    public async Task<ActionResult<CollectionResponseDto<BookingResponseDto>>> Get(
        [FromQuery] Guid? participantId = null,
        [FromQuery] Guid? providerId = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetBookingsQuery(participantId, providerId, fromUtc, toUtc), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponseDto>> GetById(Guid id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetBookingByIdQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create(BookingCreateDto dto, CancellationToken ct = default)
    {
        var response = await _mediator.Send(new CreateBookingCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookingResponseDto>> Update(Guid id, BookingUpdateDto dto, CancellationToken ct = default)
        => Ok(await _mediator.Send(new UpdateBookingCommand(id, dto), ct));

    // POST /api/bookings/{id}/confirm
    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult<BookingResponseDto>> Confirm(Guid id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new ConfirmBookingCommand(id), ct));

    // POST /api/bookings/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<BookingResponseDto>> Cancel(Guid id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new CancelBookingCommand(id), ct));
}
