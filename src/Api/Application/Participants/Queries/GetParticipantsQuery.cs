using Api.Dtos.Participants;
using MediatR;

namespace Api.Application.Participants;

public sealed record GetParticipantsQuery(int Page = 1, int PageSize = 20, string? Q = null) : IRequest<ParticipantListResponseDto>;
