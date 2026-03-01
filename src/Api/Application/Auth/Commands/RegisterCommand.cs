using Api.Dtos.Auth;
using MediatR;

namespace Api.Application.Auth;

public sealed record RegisterCommand(RegisterDto Dto) : IRequest<RegisterResult>;
