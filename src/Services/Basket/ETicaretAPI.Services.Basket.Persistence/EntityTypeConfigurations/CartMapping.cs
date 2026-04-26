using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class CartMapping : BaseEntityConfiguration<Cart>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.CouponId)
            .HasColumnName("coupon_id");

        builder.Property(x => x.Subtotal)
            .HasColumnName("subtotal")
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0m);

        builder.Property(x => x.DiscountAmount)
            .HasColumnName("discount_amount")
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0m);

        builder.Property(x => x.Total)
            .HasColumnName("total")
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0m);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_carts_user_id");

        builder.HasOne(x => x.Coupon)
            .WithMany(x => x.Carts)
            .HasForeignKey(x => x.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
