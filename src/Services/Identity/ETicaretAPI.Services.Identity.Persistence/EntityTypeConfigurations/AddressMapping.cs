using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Identity.Persistence.EntityTypeConfigurations;

public class AddressMapping : BaseEntityConfiguration<Address>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(100);

        builder.Property(x => x.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        builder.Property(x => x.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(x => x.District)
            .HasColumnName("district")
            .HasMaxLength(100);

        builder.Property(x => x.FullAddress)
            .HasColumnName("full_address")
            .HasMaxLength(500);

        builder.Property(x => x.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(20);

        builder.Property(x => x.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_addresses_user_id");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
