using Domain.Entities;

using Infrastructure.Constants;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
}