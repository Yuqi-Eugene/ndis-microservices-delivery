using MediatR;

namespace Api.Application.Providers;

public sealed record DeleteProviderCommand(Guid Id) : IRequest<Unit>;
