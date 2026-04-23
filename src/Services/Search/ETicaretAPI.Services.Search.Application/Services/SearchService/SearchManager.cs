using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Infrastructure.ApiService;
using ETicaretAPI.Services.Search.Application.Repositories;
using ETicaretAPI.Services.Search.Domain.Documents;
using ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Services.Search.Application.Services.SearchService;

public sealed class SearchManager : ISearchService
{
    private readonly IProductSearchRepository _searchRepository;
    private readonly IRestApiService _apiService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SearchManager> _logger;

    private const int IndexPageSize = 100;

    public SearchManager(
        IProductSearchRepository searchRepository,
        IRestApiService apiService,
        IConfiguration configuration,
        ILogger<SearchManager> logger)
    {
        _searchRepository = searchRepository;
        _apiService = apiService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>> SearchAsync(
        ProductSearchFilterDto filter, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter.Query))
            return ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>.Fail(
                "Search query cannot be empty.", 400, "SEARCH_QUERY_EMPTY");

        _logger.LogInformation("Searching products with query: {Query}, page: {Page}", filter.Query, filter.Page);

        var result = await _searchRepository.SearchAsync(filter, cancellationToken);

        return ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>.Success(result);
    }

    public async Task<ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>> FilterAsync(
        ProductSearchFilterDto filter, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Filtering products - Category: {Category}, Brand: {Brand}, Price: {MinPrice}-{MaxPrice}",
            filter.Category, filter.Brand, filter.MinPrice, filter.MaxPrice);

        var result = await _searchRepository.SearchAsync(filter, cancellationToken);

        return ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>.Success(result);
    }

    public async Task<ApiResponse<List<SearchSuggestionDto>>> SuggestAsync(
        string query, int size = 5, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return ApiResponse<List<SearchSuggestionDto>>.Fail(
                "Suggest query cannot be empty.", 400, "SUGGEST_QUERY_EMPTY");

        _logger.LogInformation("Getting suggestions for: {Query}", query);

        var suggestions = await _searchRepository.SuggestAsync(query, size, cancellationToken);

        return ApiResponse<List<SearchSuggestionDto>>.Success(suggestions);
    }

    public async Task<ApiResponse<int>> IndexAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var catalogUrl = _configuration["ServiceUrls:CatalogUrl"]
            ?? throw new InvalidOperationException("ServiceUrls:CatalogUrl is not configured.");

        var endpoint = $"{catalogUrl}/api/catalog/products/getAllFilter";

        int currentPage = 1;
        int totalIndexed = 0;
        int pageCount;

        _logger.LogInformation("Starting full product reindex from Catalog service.");

        do
        {
            var queryParams = new { Page = currentPage, PageSize = IndexPageSize, IsActive = (bool?)null };

            var pagedResult = await _apiService.GetDataResultAsync<PagedResult<CatalogProductDto>>(endpoint, queryParams);

            if (pagedResult is null || pagedResult.Results.Count == 0)
                break;

            pageCount = pagedResult.PageCount;

            var documents = pagedResult.Results.Select(MapToDocument).ToList();

            await _searchRepository.BulkIndexAsync(documents, cancellationToken);

            totalIndexed += documents.Count;
            _logger.LogInformation("Indexed page {Page}/{PageCount} — {Count} products.", currentPage, pageCount, documents.Count);

            currentPage++;
        }
        while (currentPage <= pageCount);

        _logger.LogInformation("Reindex completed. Total products indexed: {Total}", totalIndexed);

        return ApiResponse<int>.Success(totalIndexed, $"{totalIndexed} products indexed successfully.");
    }

    private static ProductDocument MapToDocument(CatalogProductDto dto) => new()
    {
        Id = dto.Id,
        Code = dto.Code,
        Name = dto.Name,
        Description = dto.Description,
        Slug = dto.Slug,
        Price = dto.Price,
        StockQuantity = dto.StockQuantity,
        IsActive = dto.IsActive,
        IsFeatured = dto.IsFeatured,
        CategoryId = dto.CategoryId,
        CategoryName = dto.CategoryName,
        BrandId = dto.BrandId,
        BrandName = dto.BrandName,
        ImageUrls = dto.ImageUrls ?? [],
        CreatedAt = dto.CreatedAt,
        ModifiedAt = dto.ModifiedAt,
    };

    private sealed class CatalogProductDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public List<string> ImageUrls { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
