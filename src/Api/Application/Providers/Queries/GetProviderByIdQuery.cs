using Api.Dtos.Providers;
using MediatR;

namespace Api.Application.Providers;

public sealed record GetProviderByIdQuery(Guid Id) : IRequest<ProviderResponseDto>;
