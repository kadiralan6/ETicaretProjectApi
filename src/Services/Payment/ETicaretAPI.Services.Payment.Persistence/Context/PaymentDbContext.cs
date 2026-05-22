using System.Reflection;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Services.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Payment.Persistence.Context;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<PaymentDetail> PaymentDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Set legacy timestamp behavior for Npgsql globally
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ETicaretProjectDb_Payment;Username=postgres;Password=EticaretAPI123!");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
