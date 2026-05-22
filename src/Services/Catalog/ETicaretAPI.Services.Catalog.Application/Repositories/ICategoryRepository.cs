using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Repositories;

public interface ICategoryRepository : IEntityRepository<Category>
{
    /// <summary>
    /// Slug ile kategori breadcrumb'ını getirir. Kökten yaprağa sıralı.
    /// </summary>
    Task<List<BreadcrumbItemDto>> GetBreadcrumbBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Popüler kategorileri getirir (aktif, ürün sayısına göre sıralı).
    /// </summary>
    Task<List<CategorySummaryDto>> GetPopularCategoriesAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Slug ile kategori bilgisini getirir (sadece summary bilgisi).
    /// </summary>
    Task<CategorySummaryDto?> GetCategorySummaryBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Slug ile kategori ID'sini getirir.
    /// </summary>
    Task<int?> GetCategoryIdBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir slug'ın başka bir kategori tarafından kullanılıp kullanılmadığını kontrol eder.
    /// </summary>
    Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}