using Domain.Entities;
using Domain.Entities.Enums;

using Infrastructure.Database;

using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Seeders;

public class PermissionSeeder : EntitySeeder<Permission>
{
    public override uint Order => 0;

    public override async Task<int> SeedAsync(ApplicationDbContext context, int seed)
    {
        if (await context.Permissions.AnyAsync()) return 0;

        await context.Permissions.AddRangeAsync(ManualData());

        return await context.SaveChangesAsync();
    }

    protected override IEnumerable<Permission> ManualData()
        => Enum.GetValues<PermissionName>().Select(p => new Permission { Name = p });
}