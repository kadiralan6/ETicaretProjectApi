using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class CartItemsMapping : BaseEntityConfiguration<CartItems>
{
    protected override void ConfigureEntity(EntityTypeBuilder<CartItems> builder)
    {
        builder.ToTable("cart_items");

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.CouponId)
            .HasColumnName("coupon_id");

        builder.Property(x => x.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.UserId, x.ProductId })
            .HasDatabaseName("ix_cart_items_user_product");

        builder.HasOne(x => x.Coupon)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.CouponId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}