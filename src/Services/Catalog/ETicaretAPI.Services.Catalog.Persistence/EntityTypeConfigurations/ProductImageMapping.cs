using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Catalog.Persistence.EntityTypeConfigurations;

public class ProductImageMapping : BaseEntityConfiguration<ProductImage>
{
  protected override void ConfigureEntity(EntityTypeBuilder<ProductImage> builder)
  {
    builder.ToTable("product_images");

    builder.Property(x => x.Url)
        .HasMaxLength(500)
        .IsRequired();

    builder.Property(x => x.IsCover)
        .HasDefaultValue(false);
  }
}
