using System.Security.Authentication;
using Api.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Api.Application.Auth;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwt;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = request.Dto.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            throw new AuthenticationException("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            throw new AuthenticationException("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwt.CreateToken(user.Id, user.Email ?? email, roles);

        return new LoginResult(token, user.Email ?? email, roles.ToArray());
    }
}
