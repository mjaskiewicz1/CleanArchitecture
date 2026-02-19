using Application.Common;

using Domain.Entities;

namespace Application.Users.Dtos;

public sealed record UserProfileResponse : BaseResponse, IResponse<User, UserProfileResponse>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public DateTimeOffset? LastLogin { get; init; }

    public IEnumerable<UserPermissionResponse> Permissions { get; private init; } = [];

    public static UserProfileResponse FromEntity(User entity)
    {
        return new UserProfileResponse
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