using Api.Dtos;
using Api.Dtos.Providers;
using MediatR;

namespace Api.Application.Providers;

public sealed record GetProvidersQuery() : IRequest<CollectionResponseDto<ProviderResponseDto>>;
