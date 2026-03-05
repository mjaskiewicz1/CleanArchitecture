namespace Web.Api;

public static class PolicyNames
{
    public const string UserRead = nameof(Domain.Entities.Enums.PermissionName.UserRead);
    public const string UserCreate = nameof(Domain.Entities.Enums.PermissionName.UserCreate);
    public const string UserUpdate = nameof(Domain.Entities.Enums.PermissionName.UserUpdate);
    public const string UserDelete = nameof(Domain.Entities.Enums.PermissionName.UserDelete);
}