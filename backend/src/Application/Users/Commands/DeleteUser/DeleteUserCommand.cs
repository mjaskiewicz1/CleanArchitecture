using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(ulong Id) : IRequest<Result>;