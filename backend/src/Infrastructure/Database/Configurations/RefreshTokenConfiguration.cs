using Domain.Constants;
using Domain.Entities;

using Infrastructure.Database.Configurations.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class RefreshTokenConfiguration : DbEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.Property(static x => x.Token).IsRequired().HasMaxLength(EntityConstraintsValues.TokenLength);
        builder.Property(static x => x.ExpiresAtUtc).IsRequired();

        builder.HasIndex(static x => x.Token).IsUnique();

        builder.HasOne(static x => x.User)
            .WithMany(static y => y.RefreshTokens)
            .HasForeignKey(static x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}