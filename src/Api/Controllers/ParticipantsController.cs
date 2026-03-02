using Api.Application.Participants;
using Api.Dtos;
using Api.Dtos.Participants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

//this is attributes
[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParticipantsController(IMediator mediator) => _mediator = mediator;

    // GET /api/participants?page=1&pageSize=20&q=gene
    [HttpGet]
    public async Task<ActionResult<ParticipantListResponseDto>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetParticipantsQuery(page, pageSize, q), ct));

    // GET /api/participants/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ParticipantResponseDto>> GetById(Guid id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetParticipantByIdQuery(id), ct));

    // POST /api/participants
    [HttpPost]
    public async Task<ActionResult<ParticipantResponseDto>> Create(ParticipantCreateDto dto, CancellationToken ct = default)
    {
        var response = await _mediator.Send(new CreateParticipantCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    // PUT /api/participants/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ParticipantResponseDto>> Update(Guid id, ParticipantUpdateDto dto, CancellationToken ct = default)
        => Ok(await _mediator.Send(new UpdateParticipantCommand(id, dto), ct));

    // DELETE /api/participants/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteParticipantCommand(id), ct);
        return NoContent();
    }
}
