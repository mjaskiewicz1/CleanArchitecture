using Application.Common;

using Domain.Entities;

namespace Application.Users.Dtos;

public sealed record UserResponse : BaseResponse, IResponse<User, UserResponse>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public DateTimeOffset? LastLogin { get; init; }
    
    public IEnumerable<UserPermissionResponse> Permissions { get; init; } = [];

    public static UserResponse FromEntity(User entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            LastLogin = entity.LastLogin,
            Permissions = entity.UserPermissions.Select(UserPermissionResponse.FromEntity)
        };
    }
}