using Domain.Shared;

namespace Domain.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound("User not found");
    public static readonly Error EmailAlreadyExists = Error.Conflict("User with this email already exists");
    public static readonly Error PermissionsNotFound = Error.BadRequest("One or more permissions do not exist");
    public static readonly Error InvalidToken = Error.BadRequest("Invalid or expired token");
}