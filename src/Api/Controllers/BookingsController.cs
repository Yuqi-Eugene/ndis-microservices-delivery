using Api.Application.Bookings;
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
    public async Task<ActionResult<IEnumerable<Api.Domain.Entities.Booking>>> Get(
        [FromQuery] Guid? participantId = null,
        [FromQuery] Guid? providerId = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null)
        => Ok(await _mediator.Send(new GetBookingsQuery(participantId, providerId, fromUtc, toUtc)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Api.Domain.Entities.Booking>> GetById(Guid id)
        => Ok(await _mediator.Send(new GetBookingByIdQuery(id)));

    [HttpPost]
    public async Task<ActionResult<Api.Domain.Entities.Booking>> Create(BookingCreateDto dto)
    {
        var entity = await _mediator.Send(new CreateBookingCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Api.Domain.Entities.Booking>> Update(Guid id, BookingUpdateDto dto)
        => Ok(await _mediator.Send(new UpdateBookingCommand(id, dto)));

    // POST /api/bookings/{id}/confirm
    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult<Api.Domain.Entities.Booking>> Confirm(Guid id)
        => Ok(await _mediator.Send(new ConfirmBookingCommand(id)));

    // POST /api/bookings/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<Api.Domain.Entities.Booking>> Cancel(Guid id)
        => Ok(await _mediator.Send(new CancelBookingCommand(id)));
}
