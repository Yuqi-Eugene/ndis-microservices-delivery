using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Claims;

public sealed record GetClaimsQuery() : IRequest<List<Claim>>;
