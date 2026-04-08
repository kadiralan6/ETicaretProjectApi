using ETicaretAPI.Common.Persistence.Context;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Persistence.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Catalog.Persistence.Context;

public class CatalogDbContext : BaseDbContext
{
  public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Brand> Brands => Set<Brand>();
  public DbSet<ProductImage> ProductImages => Set<ProductImage>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfiguration(new CategoryMapping());
    modelBuilder.ApplyConfiguration(new BrandMapping());
    modelBuilder.ApplyConfiguration(new ProductMapping());
    modelBuilder.ApplyConfiguration(new ProductImageMapping());
  }
}
