using ETicaretAPI.Common.Persistence.Context;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Basket.Persistence.Context;

public class BasketDbContext : BaseDbContext
{
  public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

  public DbSet<Campaign> Campaigns => Set<Campaign>();
  public DbSet<Coupon> Coupons => Set<Coupon>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Campaign>(entity =>
    {
      entity.HasKey(c => c.Id);
      entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
      entity.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
      entity.Property(c => c.MinimumOrderAmount).HasColumnType("decimal(18,2)");
    });

    modelBuilder.Entity<Coupon>(entity =>
    {
      entity.HasKey(c => c.Id);
      entity.Property(c => c.Code).IsRequired().HasMaxLength(50);
      entity.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
      entity.Property(c => c.MinimumOrderAmount).HasColumnType("decimal(18,2)");
      entity.HasIndex(c => c.Code).IsUnique();
    });
  }
}
