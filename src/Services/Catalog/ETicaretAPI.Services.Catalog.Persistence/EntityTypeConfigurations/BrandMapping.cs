using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Catalog.Persistence.EntityTypeConfigurations;

public class BrandMapping : BaseEntityConfiguration<Brand>
{
  protected override void ConfigureEntity(EntityTypeBuilder<Brand> builder)
  {
    builder.ToTable("brands");

    builder.Property(x => x.Name)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(x => x.Slug)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(x => x.Description)
        .HasMaxLength(500);

    builder.Property(x => x.IsActive)
        .HasDefaultValue(true);

    builder.HasIndex(x => x.Slug)
        .IsUnique();

    builder.HasMany(x => x.Products)
        .WithOne(x => x.Brand)
        .HasForeignKey(x => x.BrandId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
