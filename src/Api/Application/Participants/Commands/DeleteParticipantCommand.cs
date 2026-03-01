using MediatR;

namespace Api.Application.Participants;

public sealed record DeleteParticipantCommand(Guid Id) : IRequest<Unit>;
