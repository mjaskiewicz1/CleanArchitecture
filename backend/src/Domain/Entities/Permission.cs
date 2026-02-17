using Domain.Entities.Enums;
using Domain.Entities.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a permission that can be assigned to users in the application.
/// </summary>
public sealed class Permission : DbEntity
{
    /// <summary>
    /// Gets the name of the permission.
    /// </summary>
    public required PermissionName Name { get; init; }


    /// <summary>
    /// Gets the collection of user-permission associations linked to this permission.
    /// </summary>
    public ICollection<UserPermission> UserPermissions { get; init; } = [];
}