using System.Security.Authentication;
using Api.Auth;
using Api.Dtos.Auth;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Api.Application.Auth;

public sealed record RegisterCommand(RegisterDto Dto) : IRequest<RegisterResult>;
public sealed record LoginCommand(LoginDto Dto) : IRequest<LoginResult>;

public sealed record RegisterResult(string Message, string Email);
public sealed record LoginResult(string Token, string Email, IReadOnlyList<string> Roles);

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken ct)
    {
        var email = request.Dto.Email.Trim().ToLowerInvariant();

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            throw new ValidationException(
                new[] { new ValidationFailure(nameof(request.Dto.Email), "User already exists.") });
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var create = await _userManager.CreateAsync(user, request.Dto.Password);
        if (!create.Succeeded)
        {
            throw new ValidationException(
                create.Errors.Select(ToValidationFailure));
        }

        await _userManager.AddToRoleAsync(user, "Provider");

        return new RegisterResult("Registered.", user.Email ?? email);
    }

    private static ValidationFailure ToValidationFailure(IdentityError error)
    {
        var propertyName = error.Code.Contains("Password", StringComparison.OrdinalIgnoreCase)
            ? nameof(RegisterDto.Password)
            : nameof(RegisterDto.Email);

        return new ValidationFailure(propertyName, error.Description);
    }
}

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

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Dto.Password).NotEmpty();
    }
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Dto.Password).NotEmpty();
    }
}
