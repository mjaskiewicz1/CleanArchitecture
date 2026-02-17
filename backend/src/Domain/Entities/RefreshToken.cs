using Domain.Entities.Generic;

namespace Domain.Entities;

/// <summary>
/// Represents a refresh token entity used for user authentication and session management.
/// </summary>
public sealed class RefreshToken : DbEntity
{
    /// <summary>
    /// Hashed refresh token value
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Refresh token expiration date
    /// </summary>
    public DateTime ExpiresAtUtc { get; init; }


    /// <summary>
    /// Associated user
    /// </summary>
    public User User { get; init; } = null!;

    /// <summary>
    /// Associated user ID
    /// </summary>
    public required ulong UserId { get; init; }
}