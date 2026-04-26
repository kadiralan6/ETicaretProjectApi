using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class CouponMapping : BaseEntityConfiguration<Coupon>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("coupons");

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(x => x.DiscountValue)
            .HasColumnName("discount_value")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.MinimumOrderAmount)
            .HasColumnName("minimum_order_amount")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.ExpirationDate)
            .HasColumnName("expiration_date")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.UsageLimit)
            .HasColumnName("usage_limit");

        builder.Property(x => x.UsageCount)
            .HasColumnName("usage_count")
            .HasDefaultValue(0);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("ix_coupons_code");
    }
}
