using Api.Dtos;
using Api.Dtos.Claims;
using MediatR;

namespace Api.Application.Claims;

public sealed record GetClaimsQuery() : IRequest<CollectionResponseDto<ClaimResponseDto>>;
