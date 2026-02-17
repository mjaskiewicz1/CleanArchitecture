using Domain.Entities.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Generic;

public abstract class DbEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : DbEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(static x => x.Id);
        builder.Property(static x => x.Id).ValueGeneratedOnAdd();
    }
}