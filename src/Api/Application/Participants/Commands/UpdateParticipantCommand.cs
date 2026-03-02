using Api.Dtos;
using Api.Dtos.Participants;
using MediatR;

namespace Api.Application.Participants;

public sealed record UpdateParticipantCommand(Guid Id, ParticipantUpdateDto Dto) : IRequest<ParticipantResponseDto>;
