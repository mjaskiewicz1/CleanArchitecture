using Domain.Entities.Generic;

using Infrastructure.Database;

namespace Infrastructure.Seeders;

public abstract class EntitySeeder<TEntity> : IEntitySeeder where TEntity : DbEntity
{
    public abstract uint Order { get; }

    public abstract Task<int> SeedAsync(ApplicationDbContext context, int seed);

    protected abstract IEnumerable<TEntity> ManualData();

    protected virtual IEnumerable<TEntity> FakerData(int seed) => [];
}