using Application.Common;

using Domain.Entities;

namespace Application.Users.Dtos;

public sealed record UserDetailsResponse : BaseResponse, IResponse<User, UserDetailsResponse>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public DateTimeOffset? LastLogin { get; init; }

    public IEnumerable<UserPermissionResponse> Permissions { get; private init; } = [];

    public static UserDetailsResponse FromEntity(User entity)
    {
        return new UserDetailsResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            LastLogin = entity.LastLogin,
            Permissions = entity.UserPermissions.Select(UserPermissionResponse.FromEntity),
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}