namespace Domain.Constants;

/// <summary>
/// Contains constants for maximum lengths of various entity properties
/// </summary>
public static class EntityConstraintsValues
{
    /// <summary>
    /// E.g. First Name, Last Name, Username and similar short name properties
    /// </summary>
    public const int NameLength = 100;

    public const int EmailLength = 256;
    public const int TokenLength = 100;
    public const int PasswordHashLength = 128;
}