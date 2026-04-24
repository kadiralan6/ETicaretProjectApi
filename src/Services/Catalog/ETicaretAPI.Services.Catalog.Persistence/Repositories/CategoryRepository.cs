using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Catalog.Persistence.Repositories;

public class CategoryRepository : EfEntityRepositoryBase<Category, CatalogDbContext>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<List<BreadcrumbItemDto>> GetBreadcrumbBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        // İlk olarak slug ile kategoriyi bul
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Slug == slug && !c.IsDeleted)
            .Select(c => new { c.Id, c.Name, c.Slug, c.ParentCategoryId })
            .FirstOrDefaultAsync(cancellationToken);

        if (category == null)
            return new List<BreadcrumbItemDto>();

        // Breadcrumb'ı oluştur — yapraktan köke gidip ters çevir
        var breadcrumb = new List<BreadcrumbItemDto>();
        breadcrumb.Add(new BreadcrumbItemDto { Name = category.Name, Slug = category.Slug });

        var parentId = category.ParentCategoryId;

        // Max 10 seviye derinlik (sonsuz döngü koruması)
        var maxDepth = 10;
        while (parentId.HasValue && maxDepth-- > 0)
        {
            var parent = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == parentId.Value && !c.IsDeleted)
                .Select(c => new { c.Id, c.Name, c.Slug, c.ParentCategoryId })
                .FirstOrDefaultAsync(cancellationToken);

            if (parent == null)
                break;

            breadcrumb.Add(new BreadcrumbItemDto { Name = parent.Name, Slug = parent.Slug });
            parentId = parent.ParentCategoryId;
        }

        // Kökten yaprağa sırala
        breadcrumb.Reverse();
        return breadcrumb;
    }

    public async Task<List<CategorySummaryDto>> GetPopularCategoriesAsync(int count, CancellationToken cancellationToken = default)
    {
        // Aktif kategoriler, ürün sayısına göre sıralı
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Take(count)
            .Select(c => new CategorySummaryDto
            {
                Name = c.Name,
                Slug = c.Slug,
                ImageUrl = c.ImageUrl
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CategorySummaryDto?> GetCategorySummaryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Slug == slug && !c.IsDeleted && c.IsActive)
            .Select(c => new CategorySummaryDto
            {
                Name = c.Name,
                Slug = c.Slug,
                ImageUrl = c.ImageUrl
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int?> GetCategoryIdBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Slug == slug && !c.IsDeleted)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.Slug == slug && !c.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}