using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using ETicaretAPI.Services.Basket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class OrderMapping : BaseEntityConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasColumnName("total_price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(100);

        builder.Property(x => x.CouponId)
            .HasColumnName("coupon_id");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(OrderStatusEnum.Pending);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_orders_user_id");

        builder.HasIndex(x => x.OrderNumber)
            .HasDatabaseName("ix_orders_order_number");
    }
}