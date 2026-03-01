using Api.Domain.Entities;
using Api.Dtos.Claims;
using MediatR;

namespace Api.Application.Claims;

public sealed record CreateClaimCommand(ClaimCreateDto Dto) : IRequest<Claim>;
