using Domain.Entities.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a permission assigned to a specific user in the application.
/// </summary>
public sealed class UserPermission : DbEntity
{
    /// <summary>
    /// Gets the identifier of the user associated with this permission entry.
    /// </summary>
    public ulong UserId { get; init; }

    /// <summary>
    /// Gets the identifier of the permission assigned to the user.
    /// </summary>
    public ulong PermissionId { get; init; }


    /// <summary>
    /// Gets the user to whom the permission is assigned.
    /// </summary>
    public User User { get; init; } = null!;

    /// <summary>
    /// Gets the permission assigned to the user.
    /// </summary>
    public Permission Permission { get; init; } = null!;
}