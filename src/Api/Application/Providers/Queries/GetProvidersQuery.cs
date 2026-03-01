using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Providers;

public sealed record GetProvidersQuery() : IRequest<List<Provider>>;
