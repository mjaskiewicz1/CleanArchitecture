using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.Logout;

public sealed record LogoutCommand(ulong UserId) : IRequest<Result>;