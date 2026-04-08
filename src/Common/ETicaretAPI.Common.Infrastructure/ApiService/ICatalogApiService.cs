using ETicaretAPI.Common.SharedLibrary.DTOs.Product;

namespace ETicaretAPI.Common.Infrastructure.ApiService;

/// <summary>
/// Catalog API calls with automatic caching support
/// Used for cross-service communication to get Product and Package information
/// </summary>
public interface ICatalogApiService
{
    #region Product Methods

    /// <summary>
    /// Gets product basic info (name, slug, description) by ID with caching
    /// </summary>
    Task<GetProductCacheDto?> GetProductByIdAsync(Guid productId);

    /// <summary>
    /// Gets multiple products basic info by IDs with caching
    /// </summary>
    Task<List<GetProductCacheDto>> GetProductsByIdsAsync(List<Guid> productIds);

    #endregion

    #region Package Methods

    /// <summary>
    /// Gets package basic info (name, slug, description) by ID with caching
    /// </summary>
    Task<GetPackageCacheDto?> GetPackageByIdAsync(Guid packageId);

    /// <summary>
    /// Gets multiple packages basic info by IDs with caching
    /// </summary>
    Task<List<GetPackageCacheDto>> GetPackagesByIdsAsync(List<Guid> packageIds);

    #endregion

    #region Cache Invalidation

    /// <summary>
    /// Invalidates product cache by ID
    /// </summary>
    Task InvalidateProductCacheAsync(Guid productId);

    /// <summary>
    /// Invalidates multiple products cache
    /// </summary>
    Task InvalidateProductsCacheAsync(List<Guid> productIds);

    /// <summary>
    /// Invalidates package cache by ID
    /// </summary>
    Task InvalidatePackageCacheAsync(Guid packageId);

    /// <summary>
    /// Invalidates multiple packages cache
    /// </summary>
    Task InvalidatePackagesCacheAsync(List<Guid> packageIds);

    #endregion
}
