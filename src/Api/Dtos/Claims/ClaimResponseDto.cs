namespace Api.Dtos.Claims;

public sealed record ClaimResponseDto(
    Guid Id,
    Guid ServiceDeliveryId,
    decimal Amount,
    string Status,
    DateTime CreatedAtUtc);
