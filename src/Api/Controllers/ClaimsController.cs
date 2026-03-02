using Api.Application.Claims;
using Api.Dtos;
using Api.Dtos.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClaimsController(IMediator mediator) => _mediator = mediator;

    // GET /api/claims
    [HttpGet]
    public async Task<ActionResult<CollectionResponseDto<ClaimResponseDto>>> Get(CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetClaimsQuery(), ct));

    // POST /api/claims
    [HttpPost]
    public async Task<ActionResult<ClaimResponseDto>> Create(ClaimCreateDto dto, CancellationToken ct = default)
        => Ok(await _mediator.Send(new CreateClaimCommand(dto), ct));
}
