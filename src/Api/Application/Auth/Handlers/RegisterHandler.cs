using Api.Auth;
using Api.Dtos.Auth;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Api.Application.Auth;

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
            throw new ValidationException(create.Errors.Select(ToValidationFailure));
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
