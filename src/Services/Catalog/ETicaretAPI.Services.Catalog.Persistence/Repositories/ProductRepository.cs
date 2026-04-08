using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Persistence.Context;

namespace ETicaretAPI.Services.Catalog.Persistence.Repositories;

public class ProductRepository : EfEntityRepositoryBase<Product, CatalogDbContext>, IProductRepository
{
  public ProductRepository(CatalogDbContext context) : base(context)
  {
  }
}
