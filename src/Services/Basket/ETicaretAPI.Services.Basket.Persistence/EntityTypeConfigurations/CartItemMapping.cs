using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class CartItemMapping : BaseEntityConfiguration<CartItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");

        builder.Property(x => x.CartId)
            .HasColumnName("cart_id")
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(x => x.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ProductSlug)
            .HasColumnName("product_slug")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500);

        builder.Property(x => x.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.HasIndex(x => new { x.CartId, x.ProductId })
            .HasDatabaseName("ix_cart_items_cart_product");
    }
}
