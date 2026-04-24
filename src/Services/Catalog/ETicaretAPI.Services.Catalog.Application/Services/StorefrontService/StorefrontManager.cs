using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Catalog.Application.Repositories;
using ETicaretAPI.Services.Catalog.Domain.DTOs.StorefrontDtos;

namespace ETicaretAPI.Services.Catalog.Application.Services.StorefrontService;

public class StorefrontManager : IStorefrontService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly ICacheService _cacheService;

    // Cache key patterns — slug bazlı
    private static string ProductDetailCacheKey(string slug) => $"storefront:product:{slug}";
    private static string CategoryProductsCacheKey(string slug, int page, int pageSize) => $"storefront:category:{slug}:p{page}:s{pageSize}";
    private static string BreadcrumbCacheKey(string slug) => $"storefront:breadcrumb:{slug}";
    private const string HomeCacheKey = "storefront:home";

    // Cache TTL'leri
    private static readonly TimeSpan ProductDetailTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan CategoryProductsTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan BreadcrumbTtl = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan HomeTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan SimilarProductsTtl = TimeSpan.FromMinutes(15);

    // Varsayılan limitler
    private const int FeaturedProductsCount = 8;
    private const int PopularCategoriesCount = 6;
    private const int MaxSimilarProductsCount = 20;

    public StorefrontManager(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICacheService cacheService,
        IProductImageRepository productImageRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _productImageRepository = productImageRepository;
    }

    public async Task<ApiResponse<ProductDetailDto>> GetProductBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return ApiResponse<ProductDetailDto>.Fail("Slug boş olamaz.", 400);

        // Cache kontrol
        var cached = await _cacheService.GetAsync<ProductDetailDto>(ProductDetailCacheKey(slug), cancellationToken);
        if (cached is not null)
            return ApiResponse<ProductDetailDto>.Success(cached);

        var product = await _productRepository.GetProductDetailBySlugAsync(slug, cancellationToken);

        if (product is null)
            throw new NotFoundException($"'{slug}' slug'ına sahip ürün bulunamadı.");

        // Breadcrumb'ı ürün response'una göm — frontend ayrı çağrı yapmaz
        if (product.Category?.Slug is not null)
        {
            product.Breadcrumbs = await _categoryRepository.GetBreadcrumbBySlugAsync(
                product.Category.Slug, cancellationToken);
        }

        // Fake rating ekle — deterministik (aynı ürün her zaman aynı değer alır)
        product.Rating = GenerateFakeRating(product.Id);

        // Cache'e yaz
        await _cacheService.SetAsync(ProductDetailCacheKey(slug), product, ProductDetailTtl, cancellationToken);

        return ApiResponse<ProductDetailDto>.Success(product);
    }

    public async Task<ApiResponse<CategoryProductsResponseDto>> GetProductsByCategorySlugAsync(
        string slug, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return ApiResponse<CategoryProductsResponseDto>.Fail("Slug boş olamaz.", 400);

        // Sayfalama sınırları
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 50) pageSize = 50;

        // Cache kontrol
        var cacheKey = CategoryProductsCacheKey(slug, page, pageSize);
        var cached = await _cacheService.GetAsync<CategoryProductsResponseDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return ApiResponse<CategoryProductsResponseDto>.Success(cached);

        // Kategori bilgisini al
        var categorySummary = await _categoryRepository.GetCategorySummaryBySlugAsync(slug, cancellationToken);
        if (categorySummary is null)
            throw new NotFoundException($"'{slug}' slug'ına sahip kategori bulunamadı.");

        // Kategori ID'sini al
        var categoryId = await _categoryRepository.GetCategoryIdBySlugAsync(slug, cancellationToken);
        if (!categoryId.HasValue)
            throw new NotFoundException($"'{slug}' slug'ına sahip kategori bulunamadı.");

        // Ürünleri getir
        var (products, totalCount) = await _productRepository.GetProductCardsByCategoryIdAsync(
            categoryId.Value, page, pageSize, cancellationToken);

        // Her ürüne fake rating ekle
        foreach (var p in products)
            p.Rating = GenerateFakeRating(p.Id);

        var response = new CategoryProductsResponseDto
        {
            Category = categorySummary,
            Products = products,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        // Cache'e yaz
        await _cacheService.SetAsync(cacheKey, response, CategoryProductsTtl, cancellationToken);

        return ApiResponse<CategoryProductsResponseDto>.Success(response);
    }

    public async Task<ApiResponse<List<BreadcrumbItemDto>>> GetBreadcrumbBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return ApiResponse<List<BreadcrumbItemDto>>.Fail("Slug boş olamaz.", 400);

        // Cache kontrol
        var cached = await _cacheService.GetAsync<List<BreadcrumbItemDto>>(BreadcrumbCacheKey(slug), cancellationToken);
        if (cached is not null)
            return ApiResponse<List<BreadcrumbItemDto>>.Success(cached);

        var breadcrumb = await _categoryRepository.GetBreadcrumbBySlugAsync(slug, cancellationToken);

        if (breadcrumb.Count == 0)
            throw new NotFoundException($"'{slug}' slug'ına sahip kategori bulunamadı.");

        // Cache'e yaz
        await _cacheService.SetAsync(BreadcrumbCacheKey(slug), breadcrumb, BreadcrumbTtl, cancellationToken);

        return ApiResponse<List<BreadcrumbItemDto>>.Success(breadcrumb);
    }

    public async Task<ApiResponse<HomePageDto>> GetHomeDataAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cacheService.GetAsync<HomePageDto>(HomeCacheKey, cancellationToken);
        if (cached is not null)
            return ApiResponse<HomePageDto>.Success(cached);

        var featuredProducts = await _productRepository.GetFeaturedProductCardsAsync(FeaturedProductsCount, cancellationToken);
        var popularCategories = await _categoryRepository.GetPopularCategoriesAsync(PopularCategoriesCount, cancellationToken);

        // N+1 yerine tek batch sorgusu: tüm ürünlerin görsellerini tek round-trip'te çek
        await EnrichProductCardsAsync(featuredProducts, cancellationToken);

        var response = new HomePageDto
        {
            FeaturedProducts = featuredProducts,
            PopularCategories = popularCategories
        };

        await _cacheService.SetAsync(HomeCacheKey, response, HomeTtl, cancellationToken);

        return ApiResponse<HomePageDto>.Success(response);
    }

    public async Task<ApiResponse<List<ProductCardDto>>> GetSimilarProductsAsync(string slug, int count, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return ApiResponse<List<ProductCardDto>>.Fail("Slug boş olamaz.", 400);

        if (count < 1) count = 8;
        if (count > MaxSimilarProductsCount) count = MaxSimilarProductsCount;

        var products = await _productRepository.GetSimilarProductCardsBySlugAsync(slug, count, cancellationToken);

        await EnrichProductCardsAsync(products, cancellationToken);

        return ApiResponse<List<ProductCardDto>>.Success(products);
    }


    private async Task EnrichProductCardsAsync(
        List<ProductCardDto> products,
        CancellationToken cancellationToken)
    {
        if (products.Count == 0) return;

        var productIds = products.Select(p => p.Id).ToList();

        // Tek batch sorgusu — tüm ürün ID'leri için görselleri bir kerede çek
        var allImages = await _productImageRepository.GetAllAsync(
            img => productIds.Contains(img.ProductId),
            cancellationToken: cancellationToken);

        // ProductId -> URL listesi eşlemesi (in-memory, O(n))
        var imageMap = allImages
            .GroupBy(img => img.ProductId)
            .ToDictionary(g => g.Key, g => g.Select(img => img.Url).Where(u => u is not null).Cast<string>().ToList());

        foreach (var product in products)
        {
            product.Rating = GenerateFakeRating(product.Id);
            product.IsInStock = product.StockQuantity > 0;
            product.ImageUrls = imageMap.TryGetValue(product.Id, out var urls) ? urls : [];
        }
    }


    private static RatingDto GenerateFakeRating(int productId)
    {
        var rng = new Random(productId * 31);
        return new RatingDto
        {
            Average = Math.Round(4.5 + rng.NextDouble() * 0.5, 1),
            Count = rng.Next(30, 101)
        };
    }
}
