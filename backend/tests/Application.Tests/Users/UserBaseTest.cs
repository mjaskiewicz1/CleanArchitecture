using Domain.Entities;

namespace Application.Tests.Users;

public abstract class UserBaseTest
{
    protected static User CreateUser(ulong id = 123, string email = "admin@admin.com", string firstName = "Admin", string lastName = "User")
        => new()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            LastLoginUtc = DateTime.UtcNow,
            UserPermissions = []
        };
}