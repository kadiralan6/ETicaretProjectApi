using System.Reflection;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Catalog.Persistence.Context;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Set legacy timestamp behavior for Npgsql globally
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ETicaretCatalogDb;Username=postgres;Password=EticaretAPI123!");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }
}
