using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

namespace Application.Users.Queries;

public sealed record GetCurrentUserQuery : IRequest<Result<UserProfileResponse>>;