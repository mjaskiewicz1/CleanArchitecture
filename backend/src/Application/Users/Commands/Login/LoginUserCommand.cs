using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;