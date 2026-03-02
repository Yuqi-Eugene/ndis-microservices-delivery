using Api.Application.Auth;
using Api.Dtos.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    // POST /api/auth/register (creates a Provider user by default)
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResult>> Register(RegisterDto dto, CancellationToken ct = default)
        => Ok(await _mediator.Send(new RegisterCommand(dto), ct));

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginDto dto, CancellationToken ct = default)
        => Ok(await _mediator.Send(new LoginCommand(dto), ct));
}
