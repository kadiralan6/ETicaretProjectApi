using ETicaretAPI.Common.Persistence.DataAccess.EntityFramework;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using ETicaretAPI.Services.Catalog.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Services.Catalog.Persistence.Repositories;

public class ProductRepository : EfEntityRepositoryBase<Product, CatalogDbContext>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<ProductDetailDto?> GetProductDetailBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Slug == slug && !p.IsDeleted && p.IsActive)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                // SEO meta alanları — backend'de üretilir, frontend ekstra işlem yapmaz
                MetaTitle = p.Name + " Fiyatı",
                MetaDescription = p.ShortDescription != null
                    ? p.ShortDescription + " en uygun fiyatla"
                    : p.Name + " en uygun fiyatla",
                Price = p.Price,
                Currency = "TRY",
                StockQuantity = p.StockQuantity,
                IsInStock = p.StockQuantity > 0,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                CreatedAt = p.CreatedAt,
                CategoryPath = p.Category != null ? p.Category.Slug : null,
                Category = p.Category != null ? new CategorySummaryDto
                {
                    Name = p.Category.Name,
                    Slug = p.Category.Slug,
                    ImageUrl = p.Category.ImageUrl
                } : null,
                Brand = p.Brand != null ? new BrandSummaryDto
                {
                    Name = p.Brand.Name,
                    Slug = p.Brand.Slug
                } : null,
                Images = p.Images
                    .Where(i => !i.IsDeleted)
                    .Select(i => new ImageDto
                    {
                        Url = i.Url,
                        AltText = i.AltText,
                        IsCover = i.IsCover
                    })
                    .ToList()
                // Breadcrumbs ve Rating → StorefrontManager'da eklenir
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(List<ProductCardDto> Products, int TotalCount)> GetProductCardsByCategoryIdAsync(
        int categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted && p.IsActive);

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductCardDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                Currency = "TRY",
                StockQuantity = p.StockQuantity,
                IsInStock = p.StockQuantity > 0,
                IsActive = p.IsActive,
                CoverImageUrl = p.Images
                    .Where(i => i.IsCover && !i.IsDeleted)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                CoverImageAlt = p.Images
                    .Where(i => i.IsCover && !i.IsDeleted)
                    .Select(i => i.AltText)
                    .FirstOrDefault(),
                CategoryName = p.Category != null ? p.Category.Name : null,
                CategorySlug = p.Category != null ? p.Category.Slug : null,
                CategoryPath = p.Category != null ? p.Category.Slug : null,
                BrandName = p.Brand != null ? p.Brand.Name : null,
                BrandSlug = p.Brand != null ? p.Brand.Slug : null
                // Rating → StorefrontManager'da eklenir
            })
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<List<ProductCardDto>> GetFeaturedProductCardsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsFeatured && p.IsActive && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(p => new ProductCardDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                Currency = "TRY",
                StockQuantity = p.StockQuantity,
                IsInStock = p.StockQuantity > 0,
                IsActive = p.IsActive,
                CoverImageUrl = p.Images
                    .Where(i => i.IsCover && !i.IsDeleted)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                CoverImageAlt = p.Images
                    .Where(i => i.IsCover && !i.IsDeleted)
                    .Select(i => i.AltText)
                    .FirstOrDefault(),
                CategoryName = p.Category != null ? p.Category.Name : null,
                CategorySlug = p.Category != null ? p.Category.Slug : null,
                CategoryPath = p.Category != null ? p.Category.Slug : null,
                BrandName = p.Brand != null ? p.Brand.Name : null,
                BrandSlug = p.Brand != null ? p.Brand.Slug : null
                // Rating → StorefrontManager'da eklenir
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.Slug == slug && !p.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
