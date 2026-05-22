using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Identity.Persistence.EntityTypeConfigurations;

public class UserMapping : BaseEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(x => x.UserName)
            .HasColumnName("user_name")
            .HasMaxLength(100);

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(500);

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        builder.Property(x => x.BirthDay)
            .HasColumnName("birth_day")
            .HasColumnType("timestamptz");

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.RefreshToken)
            .HasColumnName("refresh_token")
            .HasMaxLength(500);

        builder.Property(x => x.RefreshTokenExpiry)
            .HasColumnName("refresh_token_expiry")
            .HasColumnType("timestamptz");

        builder.Property(x => x.Roles)
            .HasColumnName("roles")
            .HasMaxLength(200)
            .HasDefaultValue("User");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");

        builder.HasIndex(x => x.UserName)
            .HasDatabaseName("ix_users_user_name");

        builder.HasMany(x => x.Addresses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
