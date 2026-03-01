using Api.Application.Providers;
using Api.Dtos.Providers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProvidersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Api.Domain.Entities.Provider>>> Get()
        => Ok(await _mediator.Send(new GetProvidersQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Api.Domain.Entities.Provider>> GetById(Guid id)
        => Ok(await _mediator.Send(new GetProviderByIdQuery(id)));

    [HttpPost]
    public async Task<ActionResult<Api.Domain.Entities.Provider>> Create(ProviderCreateDto dto)
    {
        var entity = await _mediator.Send(new CreateProviderCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Api.Domain.Entities.Provider>> Update(Guid id, ProviderUpdateDto dto)
        => Ok(await _mediator.Send(new UpdateProviderCommand(id, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProviderCommand(id));
        return NoContent();
    }
}
