using ETicaretAPI.Services.Search.Domain.Documents;
using ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;

namespace ETicaretAPI.Services.Search.Application.Repositories;

/// <summary>
/// Product arama repository interface'i.
/// Infrastructure katmanında Elasticsearch ile implemente edilir.
/// SQL Full-Text Search fallback'i ile de implemente edilebilir.
/// </summary>
public interface IProductSearchRepository
{
    Task IndexProductAsync(ProductDocument document, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(ProductDocument document, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<SearchPagedResultDto<ProductSearchResultDto>> SearchAsync(ProductSearchFilterDto filter, CancellationToken cancellationToken = default);
    Task<List<SearchSuggestionDto>> SuggestAsync(string query, int size = 5, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int productId, CancellationToken cancellationToken = default);
    Task BulkIndexAsync(IEnumerable<ProductDocument> documents, CancellationToken cancellationToken = default);
}
