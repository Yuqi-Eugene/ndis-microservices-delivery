using Api.Application.Providers;
using Api.Dtos;
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
    public async Task<ActionResult<CollectionResponseDto<ProviderResponseDto>>> Get(CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetProvidersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProviderResponseDto>> GetById(Guid id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetProviderByIdQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<ProviderResponseDto>> Create(ProviderCreateDto dto, CancellationToken ct = default)
    {
        var response = await _mediator.Send(new CreateProviderCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProviderResponseDto>> Update(Guid id, ProviderUpdateDto dto, CancellationToken ct = default)
        => Ok(await _mediator.Send(new UpdateProviderCommand(id, dto), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteProviderCommand(id), ct);
        return NoContent();
    }
}
