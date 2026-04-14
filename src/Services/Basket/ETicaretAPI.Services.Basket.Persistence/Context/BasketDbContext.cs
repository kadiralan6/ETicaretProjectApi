using System.Reflection;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Services.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Basket.Persistence.Context;

public class BasketDbContext : DbContext
{
    public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Set legacy timestamp behavior for Npgsql globally
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ETicaretBasketDb;Username=postgres;Password=EticaretAPI123!");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }
}
