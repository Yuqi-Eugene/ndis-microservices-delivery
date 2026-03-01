using Api.Domain.Entities;
using Api.Dtos.Providers;
using MediatR;

namespace Api.Application.Providers;

public sealed record UpdateProviderCommand(Guid Id, ProviderUpdateDto Dto) : IRequest<Provider>;
