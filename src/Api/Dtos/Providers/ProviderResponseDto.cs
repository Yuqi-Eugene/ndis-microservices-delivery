namespace Api.Dtos.Providers;

public sealed record ProviderResponseDto(
    Guid Id,
    string Name,
    string? Abn,
    string? ContactEmail,
    string? ContactPhone,
    string Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
