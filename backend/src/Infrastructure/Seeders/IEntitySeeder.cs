using Infrastructure.Database;

using JetBrains.Annotations;

namespace Infrastructure.Seeders;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers | ImplicitUseTargetFlags.WithInheritors)]
public interface IEntitySeeder
{
    uint Order { get; }

    Task<int> SeedAsync(ApplicationDbContext context, int seed);
}