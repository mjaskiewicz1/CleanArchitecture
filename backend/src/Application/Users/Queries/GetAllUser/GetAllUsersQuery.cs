using Application.Common;
using Application.Users.Dtos;

using Domain.Shared;

using JetBrains.Annotations;

using MediatR;

namespace Application.Users.Queries.GetAllUser;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public record GetAllUsersQuery : PaginatedRequest, IRequest<Result<IEnumerable<UserResponse>>>
{
    public ulong? Id { get; init; }
    public string? Email { get; init; }
}