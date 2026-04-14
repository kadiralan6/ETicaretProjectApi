using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Common.Infrastructure.Mappings;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity<int>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();              // EF Core için

        builder.Property(f => f.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.Property(f => f.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

        builder.Property(f => f.CreatedBy).HasColumnName("created_by");

        builder.Property(f => f.ModifiedAt).HasColumnName("modified_at")
                .HasColumnType("timestamptz");

        builder.Property(f => f.ModifiedBy).HasColumnName("modified_by");

        builder.Property(f => f.DeletedAt).HasColumnName("deleted_at")
                .HasColumnType("timestamptz");

        builder.Property(f => f.DeletedBy).HasColumnName("deleted_by");

        builder.HasQueryFilter(e => !e.IsDeleted);

        ConfigureEntity(builder); // child class'a özel alanlar burada tanımlanacak
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}