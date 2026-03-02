namespace Api.Dtos.Claims;

// A response DTO is the outward-facing API contract.
// It deliberately exposes only the fields clients need after a claim is created or queried.
public sealed record ClaimResponseDto(
    Guid Id,
    Guid ServiceDeliveryId,
    decimal Amount,
    string Status,
    DateTime CreatedAtUtc);
