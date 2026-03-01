using Api.Application.Claims;
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
    public async Task<ActionResult<IEnumerable<Api.Domain.Entities.Claim>>> Get()
        => Ok(await _mediator.Send(new GetClaimsQuery()));

    // POST /api/claims
    [HttpPost]
    public async Task<ActionResult<Api.Domain.Entities.Claim>> Create(ClaimCreateDto dto)
        => Ok(await _mediator.Send(new CreateClaimCommand(dto)));
}
