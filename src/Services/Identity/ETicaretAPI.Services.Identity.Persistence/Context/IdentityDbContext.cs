using ETicaretAPI.Services.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Identity.Persistence.Context;

/// <summary>
/// Identity DbContext. ASP.NET Core Identity ile entegre.
/// </summary>
public class IdentityDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> UsersTable => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ETicaretIdentityDb;Username=postgres;Password=EticaretAPI123!");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tablo isimlerini özelleştir
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired().HasMaxLength(50);
            entity.Property(a => a.City).IsRequired().HasMaxLength(100);
            entity.Property(a => a.FullAddress).IsRequired().HasMaxLength(500);

            entity.HasOne(a => a.User)
                  .WithMany(u => u.Addresses)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("UsersTable");
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Email).HasMaxLength(150);
            entity.Property(u => u.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.Property(a => a.Title).HasMaxLength(50);
            entity.Property(a => a.FullName).HasMaxLength(150);
            entity.Property(a => a.PhoneNumber).HasMaxLength(20);
            entity.Property(a => a.City).HasMaxLength(100);
            entity.Property(a => a.District).HasMaxLength(100);
            entity.Property(a => a.FullAddress).HasMaxLength(500);
            entity.Property(a => a.PostalCode).HasMaxLength(20);
        });
    }
}
