using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<UserDetailsResponse>>;