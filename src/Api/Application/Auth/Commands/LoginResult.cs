namespace Api.Application.Auth;

public sealed record LoginResult(string Token, string Email, IReadOnlyList<string> Roles);
