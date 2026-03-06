using Domain.Constants;
using Domain.Entities;

using Infrastructure.Database.Configurations.Generic;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class UserConfiguration : DbEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(static x => x.FirstName).IsRequired().HasMaxLength(EntityConstraintsValues.NameLength);
        builder.Property(static x => x.LastName).IsRequired().HasMaxLength(EntityConstraintsValues.NameLength);
        builder.Property(static x => x.Email).IsRequired().HasMaxLength(EntityConstraintsValues.EmailLength);
        builder.Property(static x => x.PasswordHash).IsRequired(false)
            .HasMaxLength(EntityConstraintsValues.PasswordHashLength);
        builder.Property(static x => x.LastLoginUtc).IsRequired(false);
        builder.Property(static x => x.PasswordResetTokenExpiryUtc).IsRequired(false);
        builder.Property(static x => x.PasswordResetToken).HasMaxLength(EntityConstraintsValues.TokenLength)
            .IsRequired(false);

        builder.HasIndex(rt => rt.Email).IsUnique();
    }
}