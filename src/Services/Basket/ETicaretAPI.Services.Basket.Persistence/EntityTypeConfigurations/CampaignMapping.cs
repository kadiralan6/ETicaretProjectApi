using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Basket.Persistence.EntityTypeConfigurations;

public class CampaignMapping : BaseEntityConfiguration<Campaign>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("campaigns");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
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

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.IsValid)
            .HasColumnName("is_valid")
            .HasDefaultValue(true);

        builder.Property(x => x.UsageLimit)
            .HasColumnName("usage_limit");

        builder.Property(x => x.UsageCount)
            .HasColumnName("usage_count")
            .HasDefaultValue(0);

        builder.HasIndex(x => new { x.StartDate, x.EndDate })
            .HasDatabaseName("ix_campaigns_dates");
    }
}
