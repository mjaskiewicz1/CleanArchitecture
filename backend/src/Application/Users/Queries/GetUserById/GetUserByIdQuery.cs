using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(ulong Id) : IRequest<Result<UserDetailsResponse>>;