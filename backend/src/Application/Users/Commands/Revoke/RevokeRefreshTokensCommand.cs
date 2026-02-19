using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.Revoke;

public sealed record RevokeRefreshTokensCommand : IRequest<Result>;