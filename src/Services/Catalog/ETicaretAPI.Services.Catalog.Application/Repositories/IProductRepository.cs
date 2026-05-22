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

    /// <summary>
    /// ID ile ürünü Category→ParentCategory ve Brand dahil getirir.
    /// GetWithAsNoTrackingAsync'in Include array limitasyonunu (EF Core 8 Convert hatası) aşmak için
    /// proper Include/ThenInclude zinciriyle sorgu yapar.
    /// </summary>
    Task<Product?> GetProductWithRelationsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Slug'ı verilen ürünle aynı kategorideki benzer ürün kartlarını getirir.
    /// Mevcut ürün sonuçtan hariç tutulur. Cover image projection'a dahil.
    /// </summary>
    Task<List<ProductCardDto>> GetSimilarProductCardsBySlugAsync(string slug, int count, CancellationToken cancellationToken = default);
}
