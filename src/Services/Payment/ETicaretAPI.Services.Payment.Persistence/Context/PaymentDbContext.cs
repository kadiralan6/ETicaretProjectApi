using ETicaretAPI.Common.Persistence.Context;
using ETicaretAPI.Services.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Payment.Persistence.Context;

public class PaymentDbContext : BaseDbContext
{
  public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

  public DbSet<Order> Orders => Set<Order>();
  public DbSet<OrderItem> OrderItems => Set<OrderItem>();
  public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Order>(entity =>
    {
      entity.HasKey(o => o.Id);
      entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
      entity.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
      entity.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
      entity.Property(o => o.FinalPrice).HasColumnType("decimal(18,2)");
      entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
      entity.Property(o => o.ShippingCity).IsRequired().HasMaxLength(100);
      entity.HasIndex(o => o.OrderNumber).IsUnique();
      entity.HasIndex(o => o.UserId);
    });

    modelBuilder.Entity<OrderItem>(entity =>
    {
      entity.HasKey(oi => oi.Id);
      entity.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
      entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

      entity.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<PaymentTransaction>(entity =>
    {
      entity.HasKey(pt => pt.Id);
      entity.Property(pt => pt.TransactionId).IsRequired().HasMaxLength(100);
      entity.Property(pt => pt.Amount).HasColumnType("decimal(18,2)");
      entity.HasIndex(pt => pt.TransactionId).IsUnique();

      entity.HasOne(pt => pt.Order)
                .WithOne(o => o.PaymentTransaction)
                .HasForeignKey<PaymentTransaction>(pt => pt.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
    });
  }
}
