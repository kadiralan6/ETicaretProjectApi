using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Catalog.Persistence.EntityTypeConfigurations;

public class ProductMapping : BaseEntityConfiguration<Product>
{
  protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
  {
    builder.ToTable("products");

    builder.Property(x => x.Name)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(x => x.Code)
        .HasMaxLength(50);

    builder.Property(x => x.Slug)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(x => x.Description)
        .HasMaxLength(2000);

    builder.Property(x => x.ImageUrl)
        .HasMaxLength(500);

    builder.Property(x => x.Price)
        .HasColumnType("decimal(18,2)");

    builder.Property(x => x.IsActive)
        .HasDefaultValue(true);

    builder.Property(x => x.IsFeatured)
        .HasDefaultValue(false);

    builder.HasIndex(x => x.Slug)
        .IsUnique();

    builder.HasOne(x => x.Category)
        .WithMany()
        .HasForeignKey(x => x.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(x => x.Images)
        .WithOne(x => x.Product)
        .HasForeignKey(x => x.ProductId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
