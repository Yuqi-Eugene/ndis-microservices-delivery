using Api.Dtos;
using Api.Dtos.Participants;
using MediatR;

namespace Api.Application.Participants;

public sealed record CreateParticipantCommand(ParticipantCreateDto Dto) : IRequest<ParticipantResponseDto>;
