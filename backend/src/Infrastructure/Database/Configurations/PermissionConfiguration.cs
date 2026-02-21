using Domain.Entities;

using Infrastructure.Database.Configurations.Generic;
using Infrastructure.Database.Extensions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class PermissionConfiguration : DbEntityConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.HasKey(static x => x.Id);

        builder.Property(static x => x.Name).IsRequired().HasMaxLengthFromEnum();

        builder.HasIndex(static x => x.Name).IsUnique();
    }
}