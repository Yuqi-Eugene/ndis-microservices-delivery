using Api.Application.Participants;
using Api.Dtos;
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
    public async Task<ActionResult<ParticipantListResult>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null)
        => Ok(await _mediator.Send(new GetParticipantsQuery(page, pageSize, q)));

    // GET /api/participants/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Api.Domain.Entities.Participant>> GetById(Guid id)
        => Ok(await _mediator.Send(new GetParticipantByIdQuery(id)));

    // POST /api/participants
    [HttpPost]
    public async Task<ActionResult<Api.Domain.Entities.Participant>> Create(ParticipantCreateDto dto)
    {
        var entity = await _mediator.Send(new CreateParticipantCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    // PUT /api/participants/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Api.Domain.Entities.Participant>> Update(Guid id, ParticipantUpdateDto dto)
        => Ok(await _mediator.Send(new UpdateParticipantCommand(id, dto)));

    // DELETE /api/participants/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteParticipantCommand(id));
        return NoContent();
    }
}
