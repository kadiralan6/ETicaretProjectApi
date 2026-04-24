using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Catalog.Persistence.Repositories;

public class BrandRepository : EfEntityRepositoryBase<Brand, CatalogDbContext>, IBrandRepository
{
    public BrandRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Brands
            .AsNoTracking()
            .Where(b => b.Slug == slug && !b.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
