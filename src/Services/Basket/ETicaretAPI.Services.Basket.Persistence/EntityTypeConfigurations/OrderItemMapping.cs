using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class OrderItemMapping : BaseEntityConfiguration<OrderItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

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

        builder.Property(x => x.Discount)
            .HasColumnName("discount")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.TotalPrice)
            .HasColumnName("total_price")
            .HasColumnType("numeric(18,2)");

        builder.Property(x => x.TotalNetPrice)
            .HasColumnName("total_net_price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.VatAmount)
            .HasColumnName("vat_amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CouponId)
            .HasColumnName("coupon_id");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_order_items_user_id");

        builder.HasIndex(x => x.OrderNumber)
            .HasDatabaseName("ix_order_items_order_number");
    }
}