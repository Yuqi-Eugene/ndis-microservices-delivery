using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Participants;

public sealed record GetParticipantByIdQuery(Guid Id) : IRequest<Participant>;
