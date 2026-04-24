using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

namespace ETicaretAPI.Services.Catalog.Application.Services.StorefrontService;

/// <summary>
/// Storefront (müşteri tarafı) API için service interface.
/// Frontend tek çağrıda ihtiyacı olan tüm veriyi alır.
/// </summary>
public interface IStorefrontService
{
    /// <summary>
    /// Slug ile ürün detayını getirir — category, brand, images dahil.
    /// GET /api/products/{slug}
    /// </summary>
    Task<ApiResponse<ProductDetailDto>> GetProductBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kategoriye ait ürünleri sayfalı getirir.
    /// GET /api/categories/{slug}/products?page=1&pageSize=20
    /// </summary>
    Task<ApiResponse<CategoryProductsResponseDto>> GetProductsByCategorySlugAsync(
        string slug, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kategori breadcrumb hiyerarşisini getirir.
    /// GET /api/categories/breadcrumb/{slug}
    /// </summary>
    Task<ApiResponse<List<BreadcrumbItemDto>>> GetBreadcrumbBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ana sayfa verisini getirir — featured products + popular categories.
    /// GET /api/home
    /// </summary>
    Task<ApiResponse<HomePageDto>> GetHomeDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ürün detay sayfasının altına "Benzer Ürünler" bölümü için.
    /// Aynı kategoriden, mevcut ürün hariç, en fazla <paramref name="count"/> ürün döner.
    /// </summary>
    Task<ApiResponse<List<ProductCardDto>>> GetSimilarProductsAsync(string slug, int count, CancellationToken cancellationToken = default);
}
