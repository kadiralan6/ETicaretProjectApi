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
            .HasColumnName("url")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.AltText)
            .HasColumnName("alt_text")
            .HasMaxLength(200);

        builder.Property(x => x.IsCover)
            .HasColumnName("is_cover")
            .HasDefaultValue(false);

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id");

        builder.HasOne(x => x.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
