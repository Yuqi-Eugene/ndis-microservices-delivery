using Api.Domain.Entities;
using Api.Dtos;
using MediatR;

namespace Api.Application.Participants;

public sealed record UpdateParticipantCommand(Guid Id, ParticipantUpdateDto Dto) : IRequest<Participant>;
