using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;

namespace ETicaretAPI.Services.Search.Application.Services.SearchService;

public interface ISearchService
{
    Task<ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>> SearchAsync(
        ProductSearchFilterDto filter, CancellationToken cancellationToken = default);

    Task<ApiResponse<SearchPagedResultDto<ProductSearchResultDto>>> FilterAsync(
        ProductSearchFilterDto filter, CancellationToken cancellationToken = default);

    Task<ApiResponse<List<SearchSuggestionDto>>> SuggestAsync(
        string query, int size = 5, CancellationToken cancellationToken = default);

    Task<ApiResponse<int>> IndexAllProductsAsync(CancellationToken cancellationToken = default);
}
