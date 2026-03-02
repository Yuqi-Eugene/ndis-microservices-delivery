namespace Api.Dtos.ServiceDeliveries;

public sealed record ServiceDeliveryResponseDto(
    Guid Id,
    Guid BookingId,
    DateTime ActualStartUtc,
    int ActualDurationMinutes,
    string Status,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    string OwnerUserId,
    string? OwnerEmail);
