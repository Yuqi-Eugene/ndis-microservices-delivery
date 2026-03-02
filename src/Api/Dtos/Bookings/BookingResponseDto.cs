namespace Api.Dtos.Bookings;

public sealed record BookingResponseDto(
    Guid Id,
    Guid ParticipantId,
    Guid ProviderId,
    DateTime ScheduledStartUtc,
    int DurationMinutes,
    string ServiceType,
    string Status,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
