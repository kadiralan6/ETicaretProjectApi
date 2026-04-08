using ETicaretAPI.Common.Application.Constants;
using ETicaretAPI.Common.Infrastructure.Cache;
using ETicaretAPI.Common.Infrastructure.Configuration;
using ETicaretAPI.Common.SharedLibrary.DTOs.Product;

namespace ETicaretAPI.Common.Infrastructure.ApiService;

public class CatalogApiService : ICatalogApiService
{
    private readonly ICacheService _cache;
    private readonly IRestApiService _apiService;
    private readonly string _catalogApiUrl;

    // Cache TTL configurations - Optimized for rarely changing catalog data
    private static readonly TimeSpan ProductCacheTTL = TimeSpan.FromHours(8);
    private static readonly TimeSpan PackageCacheTTL = TimeSpan.FromHours(8);

    public CatalogApiService(
        ICacheService cache,
        IRestApiService apiService)
    {
        _cache = cache;
        _apiService = apiService;
        _catalogApiUrl = CoreConfig.GetValue<string>("ExternalApi:CatalogApi:Url");
    }

    #region Product Methods

    public async Task<GetProductCacheDto?> GetProductByIdAsync(Guid productId)
    {
        // Always fetch from API to get fresh data (e.g., SerialNumber)
        Console.WriteLine($"[API CALL] Fetching product directly from API: {productId}");
        var apiUrl = $"{_catalogApiUrl}{string.Format(CatalogApiEndpoints.GetProductCacheById, productId)}";
        var productFromApi = await _apiService.GetDataResultAsync<GetProductCacheDto>(apiUrl);

        return productFromApi;
    }

    public async Task<List<GetProductCacheDto>> GetProductsByIdsAsync(List<Guid> productIds)
    {
        // Boş veya null liste kontrolü
        if (productIds == null || !productIds.Any())
            return new List<GetProductCacheDto>();

        var cachedProducts = new List<GetProductCacheDto>();
        var notFoundInCacheIds = new List<Guid>();

        // 1. Adım: Her product için cache'i kontrol et
        var distinctProductIds = productIds.Distinct().ToList();
        foreach (var productId in distinctProductIds)
        {
            var cacheKey = GetProductCacheKey(productId);
            var cachedProduct = await _cache.GetAsync<GetProductCacheDto>(cacheKey);

            if (cachedProduct != null)
            {
                Console.WriteLine($"[CACHE HIT] Product found in cache: {cacheKey}");
                cachedProducts.Add(cachedProduct);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Product not in cache: {cacheKey}");
                notFoundInCacheIds.Add(productId);
            }
        }

        // 2. Adım: Cache'de bulunamayan product'ları API'den çek
        if (notFoundInCacheIds.Any())
        {
            Console.WriteLine($"[API CALL] Fetching {notFoundInCacheIds.Count} products from API");
            var apiUrl = $"{_catalogApiUrl}{CatalogApiEndpoints.GetProductCacheByIds}";
            var productsFromApi = await _apiService.PostDataResultAsync<List<GetProductCacheDto>>(apiUrl, notFoundInCacheIds);

            // 3. Adım: API'den gelen product'ları cache'e kaydet ve sonuç listesine ekle
            if (productsFromApi != null && productsFromApi.Any())
            {
                foreach (var product in productsFromApi)
                {
                    var cacheKey = GetProductCacheKey(product.Id);
                    await _cache.SetAsync(cacheKey, product, ProductCacheTTL);
                    Console.WriteLine($"[CACHE SET] Product cached: {cacheKey}");
                    cachedProducts.Add(product);
                }
            }
        }

        Console.WriteLine($"[RESULT] Returning {cachedProducts.Count} products (Cache: {distinctProductIds.Count - notFoundInCacheIds.Count}, API: {notFoundInCacheIds.Count})");
        return cachedProducts;
    }

    #endregion

    #region Package Methods

    public async Task<GetPackageCacheDto?> GetPackageByIdAsync(Guid packageId)
    {
        var cacheKey = GetPackageCacheKey(packageId);

        // 1. Önce cache'den kontrol et
        var cachedPackage = await _cache.GetAsync<GetPackageCacheDto>(cacheKey);
        if (cachedPackage != null)
        {
            Console.WriteLine($"[CACHE HIT] Package found in cache: {cacheKey}");
            return cachedPackage;
        }

        // 2. Cache'de yoksa API'den çek
        Console.WriteLine($"[CACHE MISS] Package not in cache, fetching from API: {cacheKey}");
        var apiUrl = $"{_catalogApiUrl}{string.Format(CatalogApiEndpoints.GetPackageCacheById, packageId)}";
        var packageFromApi = await _apiService.GetDataResultAsync<GetPackageCacheDto>(apiUrl);

        // 3. API'den gelen veriyi cache'e kaydet
        if (packageFromApi != null)
        {
            await _cache.SetAsync(cacheKey, packageFromApi, PackageCacheTTL);
            Console.WriteLine($"[CACHE SET] Package cached successfully: {cacheKey}");
        }

        return packageFromApi;
    }

    public async Task<List<GetPackageCacheDto>> GetPackagesByIdsAsync(List<Guid> packageIds)
    {
        // Boş veya null liste kontrolü
        if (packageIds == null || !packageIds.Any())
            return new List<GetPackageCacheDto>();

        var cachedPackages = new List<GetPackageCacheDto>();
        var notFoundInCacheIds = new List<Guid>();

        // 1. Adım: Her package için cache'i kontrol et
        var distinctPackageIds = packageIds.Distinct().ToList();
        foreach (var packageId in distinctPackageIds)
        {
            var cacheKey = GetPackageCacheKey(packageId);
            var cachedPackage = await _cache.GetAsync<GetPackageCacheDto>(cacheKey);

            if (cachedPackage != null)
            {
                Console.WriteLine($"[CACHE HIT] Package found in cache: {cacheKey}");
                cachedPackages.Add(cachedPackage);
            }
            else
            {
                Console.WriteLine($"[CACHE MISS] Package not in cache: {cacheKey}");
                notFoundInCacheIds.Add(packageId);
            }
        }

        // 2. Adım: Cache'de bulunamayan package'ları API'den çek
        if (notFoundInCacheIds.Any())
        {
            Console.WriteLine($"[API CALL] Fetching {notFoundInCacheIds.Count} packages from API");
            var apiUrl = $"{_catalogApiUrl}{CatalogApiEndpoints.GetPackageCacheByIds}";
            var packagesFromApi = await _apiService.PostDataResultAsync<List<GetPackageCacheDto>>(apiUrl, notFoundInCacheIds);

            // 3. Adım: API'den gelen package'ları cache'e kaydet ve sonuç listesine ekle
            if (packagesFromApi != null && packagesFromApi.Any())
            {
                foreach (var package in packagesFromApi)
                {
                    var cacheKey = GetPackageCacheKey(package.Id);
                    await _cache.SetAsync(cacheKey, package, PackageCacheTTL);
                    Console.WriteLine($"[CACHE SET] Package cached: {cacheKey}");
                    cachedPackages.Add(package);
                }
            }
        }

        Console.WriteLine($"[RESULT] Returning {cachedPackages.Count} packages (Cache: {distinctPackageIds.Count - notFoundInCacheIds.Count}, API: {notFoundInCacheIds.Count})");
        return cachedPackages;
    }

    #endregion

    #region Cache Invalidation

    public async Task InvalidateProductCacheAsync(Guid productId)
    {
        await _cache.RemoveAsync(GetProductCacheKey(productId));
        Console.WriteLine($"[CACHE INVALIDATE] Product removed: {GetProductCacheKey(productId)}");
    }

    public async Task InvalidateProductsCacheAsync(List<Guid> productIds)
    {
        if (productIds == null || !productIds.Any()) return;

        foreach (var productId in productIds)
        {
            await InvalidateProductCacheAsync(productId);
        }
    }

    public async Task InvalidatePackageCacheAsync(Guid packageId)
    {
        await _cache.RemoveAsync(GetPackageCacheKey(packageId));
        Console.WriteLine($"[CACHE INVALIDATE] Package removed: {GetPackageCacheKey(packageId)}");
    }

    public async Task InvalidatePackagesCacheAsync(List<Guid> packageIds)
    {
        if (packageIds == null || !packageIds.Any()) return;

        foreach (var packageId in packageIds)
        {
            await InvalidatePackageCacheAsync(packageId);
        }
    }

    #endregion

    #region Cache Key Helpers

    private static string GetProductCacheKey(Guid productId) => $"catalog:product:{productId}";
    private static string GetPackageCacheKey(Guid packageId) => $"catalog:package:{packageId}";

    #endregion
}
