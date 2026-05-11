using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Payment.Persistence.EntityTypeConfigurations;

public class PaymentDetailMapping : BaseEntityConfiguration<PaymentDetail>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PaymentDetail> builder)
    {
        builder.ToTable("payment_details");

        builder.Property(x => x.PaymentTransactionId)
            .HasColumnName("payment_transaction_id")
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.AddressId)
            .HasColumnName("address_id")
            .IsRequired();

        builder.Property(x => x.CardNumber)
            .HasColumnName("card_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ExpiryMonth)
            .HasColumnName("expiry_month")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(x => x.ExpiryYear)
            .HasColumnName("expiry_year")
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(x => x.Cvv)
            .HasColumnName("cvv")
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(x => x.CardHolderName)
            .HasColumnName("card_holder_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.PaymentTransaction)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PaymentTransactionId);
    }
}
