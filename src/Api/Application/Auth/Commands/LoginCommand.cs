using Api.Dtos.Auth;
using MediatR;

namespace Api.Application.Auth;

public sealed record LoginCommand(LoginDto Dto) : IRequest<LoginResult>;
