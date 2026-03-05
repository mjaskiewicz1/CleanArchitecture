using Domain.Entities;
using Domain.Entities.Enums;

using Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Api.Tests;

internal sealed class TestDataSeeder(IServiceProvider services)
{
    public async Task<User> CreateUserWithPermissionsAsync(
        string? email = null,
        IEnumerable<PermissionName>? permissions = null)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        email ??= $"{Guid.NewGuid()}@test.com";

        var user = new User
        {
            Email = email,
            PasswordHash = TestWebApplicationFactory.PasswordHash,
            FirstName = "Name",
            LastName = "Surname"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();


        if (permissions is null)
            return user;
        var dbPermissions = await context.Permissions
            .Where(p => permissions.Contains(p.Name))
            .ToListAsync();

        var userPermissions = dbPermissions
            .Select(p => new UserPermission { UserId = user.Id, PermissionId = p.Id })
            .ToList();

        context.UserPermissions.AddRange(userPermissions);
        await context.SaveChangesAsync();

        return user;
    }
}