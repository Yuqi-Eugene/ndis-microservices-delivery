using Api.Domain.Entities;

namespace Api.Application.Participants;

public sealed record ParticipantListResult(int Total, int Page, int PageSize, IReadOnlyList<Participant> Items);
