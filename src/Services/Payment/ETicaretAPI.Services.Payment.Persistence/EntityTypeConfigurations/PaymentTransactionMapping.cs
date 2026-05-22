using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Payment.Persistence.EntityTypeConfigurations;

public class PaymentTransactionMapping : BaseEntityConfiguration<PaymentTransaction>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("payment_transactions");

        builder.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(x => x.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.Method)
            .HasColumnName("method")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(500);

        builder.Property(x => x.CompletedDate)
            .HasColumnName("completed_date");

        builder.HasIndex(x => x.TransactionId)
            .IsUnique();

        builder.HasIndex(x => x.OrderId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.Status);
    }
}
