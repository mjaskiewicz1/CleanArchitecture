using Application.Common;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Users.Dtos;

public sealed record UserPermissionResponse
    : BaseResponse, IResponse<UserPermission, UserPermissionResponse>
{
    public required PermissionName Name { get; init; }

    public static UserPermissionResponse FromEntity(UserPermission entity)
    {
        return new UserPermissionResponse
        {
            Id = entity.Permission.Id,
            Name = entity.Permission.Name,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}