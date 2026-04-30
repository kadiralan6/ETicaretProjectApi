using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Identity.Persistence.EntityTypeConfigurations;

public class UserAddressMapping : BaseEntityConfiguration<UserAddress>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("user_addresses");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(x => x.FullAddress)
            .HasColumnName("full_address")
            .HasMaxLength(500);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_user_addresses_user_id");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserAddresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
