using Api.Dtos.Participants;
using MediatR;

namespace Api.Application.Participants;

public sealed record GetParticipantByIdQuery(Guid Id) : IRequest<ParticipantResponseDto>;
