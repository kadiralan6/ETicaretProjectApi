using System.Reflection;
using ETicaretAPI.Services.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Identity.Persistence.Context;

/// <summary>
/// Identity DbContext. ASP.NET Core Identity ile entegre.
/// </summary>
public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> UsersTable => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Set legacy timestamp behavior for Npgsql globally
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ETicaretIdentityDb;Username=postgres;Password=EticaretAPI123!");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }
}
