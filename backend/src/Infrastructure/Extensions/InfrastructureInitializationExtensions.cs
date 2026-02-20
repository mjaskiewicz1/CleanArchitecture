using System.Reflection;

using Infrastructure.Database;
using Infrastructure.Seeders;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureInitializationExtensions
{
    private const int Seed = int.MaxValue;

    extension(IServiceProvider services)
    {
        public async Task InitializeInfrastructureAsync(bool isDevelopment,
            CancellationToken cancellationToken = default)
        {
            if (!isDevelopment)
                return;

            await using var scope = services.CreateAsyncScope();

            var dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await MigrateDatabaseAsync(dbContext, cancellationToken);
        }
    }

    #region Helpers

    private static async Task MigrateDatabaseAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Migrating database...");
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (!pendingMigrations.Any())
        {
            Console.WriteLine("No pending migrations.");
            return;
        }

        await context.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Database migration completed.");
        await SeedDatabaseAsync(context, cancellationToken);
    }

    private static async Task SeedDatabaseAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Seeding database...");

        var seederTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(IEntitySeeder).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false });

        var seeders = seederTypes
            .Select(Activator.CreateInstance)
            .OfType<IEntitySeeder>()
            .OrderBy(x => x.Order)
            .ToList();

        var totalRecords = 0;

        await using var transaction =
            await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var seeder in seeders)
            {
                var records = await seeder.SeedAsync(context, Seed);
                totalRecords += records;

                Console.WriteLine($"[Order: {seeder.Order}] {seeder.GetType().Name} added {records} records.");
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        Console.WriteLine($"Database seeding completed. Total records added: {totalRecords}");
    }

    #endregion
}