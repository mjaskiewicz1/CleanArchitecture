using Bogus;

using Domain.Entities;

using Infrastructure.Database;

using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Seeders;

public class UserSeeder : EntitySeeder<User>
{
    /// <summary>
    /// <see cref="UserSeeder"/> must be executed after:
    /// <see cref="PermissionSeeder"/>
    /// </summary>
    private const string Password =
        "784B97F150C499520BEE93F7193B8FA37BC5917ACFC5F55D1B1516B46B98023F-A65B186D61C9D4C17341A33F44317755";

    public override uint Order => 1;

    protected override List<User> ManualData() =>
    [
        new()
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@admin.com",
            PasswordHash = Password,
            LastLogin = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
        }
    ];

    protected override IEnumerable<User> FakerData(int seed)
    {
        var faker = new Faker<User>()
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.PasswordHash, _ => Password);


        return faker.UseSeed(seed).Generate(3);
    }

    public override async Task<int> SeedAsync(ApplicationDbContext context, int seed)
    {
        if (await context.Users.AnyAsync()) return 0;

        var permissions = await context.Permissions.ToListAsync();

        var users = ManualData();
        await context.Users.AddRangeAsync(users);
        await context.Users.AddRangeAsync(FakerData(seed));

        await context.SaveChangesAsync();

        // Add permissions to admin user
        var adminUser = users[0];
        var adminPermissions = permissions.ConvertAll(p => new UserPermission
        {
            UserId = adminUser.Id, PermissionId = p.Id
        });

        await context.UserPermissions.AddRangeAsync(adminPermissions);

        // Add random permissions to faker users
        var fakerUsers = await context.Users.Where(u => u.Email != "admin@admin.com").ToListAsync();
        var random = new Random(seed);

        foreach (var randomPermissions in fakerUsers.Select(user => permissions
                     .OrderBy(_ => random.Next())
                     .Take(random.Next(1, permissions.Count))
                     .Select(p => new UserPermission { UserId = user.Id, PermissionId = p.Id })
                     .ToList()))
        {
            await context.UserPermissions.AddRangeAsync(randomPermissions);
        }

        return await context.SaveChangesAsync();
    }
}