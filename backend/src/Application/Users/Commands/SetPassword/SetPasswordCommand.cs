using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.SetPassword;

public sealed record SetPasswordCommand(string Token, string Password) : IRequest<Result>;