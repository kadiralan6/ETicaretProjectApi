using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Persistence.Context;

namespace ETicaretAPI.Services.Catalog.Persistence.Repositories;

public class BrandRepository : EfEntityRepositoryBase<Brand, CatalogDbContext>, IBrandRepository
{
  public BrandRepository(CatalogDbContext context) : base(context)
  {
  }
}
