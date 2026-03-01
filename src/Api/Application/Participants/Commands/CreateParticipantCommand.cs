using Api.Domain.Entities;
using Api.Dtos;
using MediatR;

namespace Api.Application.Participants;

public sealed record CreateParticipantCommand(ParticipantCreateDto Dto) : IRequest<Participant>;
