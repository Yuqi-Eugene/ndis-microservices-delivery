namespace Api.Dtos.Participants;

public sealed record ParticipantListResponseDto(
    int Total,
    int Page,
    int PageSize,
    IReadOnlyList<ParticipantResponseDto> Items);
