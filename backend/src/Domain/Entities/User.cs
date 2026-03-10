using Domain.Entities.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents an application user with authentication and authorization data
/// </summary>
public sealed class User : DbEntity
{
    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// User's email address used as a unique identifier for authentication
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Hashed password used for authentication.
    /// Null indicates that the password has not been set yet.
    /// </summary>
    public string? PasswordHash { get; init; }

    /// <summary>
    /// Token used to authorize a password reset operation
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// Expiration date of the password reset token
    /// </summary>
    public DateTime? PasswordResetTokenExpiryUtc { get; set; }

    /// <summary>
    /// Date and time of the user's last successful login
    /// </summary>
    public DateTime? LastLoginUtc { get; set; }

    /// <summary>
    /// Collection of refresh tokens issued for this user
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; init; } = [];

    /// <summary>
    /// Collection of permissions assigned to the user
    /// </summary>
    public ICollection<UserPermission> UserPermissions { get; init; } = [];
}