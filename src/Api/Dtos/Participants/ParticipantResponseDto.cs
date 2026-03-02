namespace Api.Dtos.Participants;

public sealed record ParticipantResponseDto(
    Guid Id,
    string FullName,
    string? NdisNumber,
    string? Phone,
    string? Email,
    string Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
