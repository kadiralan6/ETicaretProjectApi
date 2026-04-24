using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;
using ETicaretAPI.Services.Catalog.Domain.Entities;

namespace ETicaretAPI.Services.Catalog.Application.Repositories;

public interface IProductRepository : IEntityRepository<Product>
{
    /// <summary>
    /// Slug ile ürün detayını getirir. Category, Brand, Images dahil — tek sorgu.
    /// </summary>
    Task<ProductDetailDto?> GetProductDetailBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir kategoriye ait ürünleri sayfalı getirir. Cover image dahil.
    /// </summary>
    Task<(List<ProductCardDto> Products, int TotalCount)> GetProductCardsByCategoryIdAsync(
        int categoryId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Öne çıkan (IsFeatured) ürünleri getirir. Ana sayfa için.
    /// </summary>
    Task<List<ProductCardDto>> GetFeaturedProductCardsAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir slug'ın başka bir ürün tarafından kullanılıp kullanılmadığını kontrol eder.
    /// excludeId: Güncelleme sırasında kendi slug'ını hariç tutmak için.
    /// </summary>
    Task<bool> IsSlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}
