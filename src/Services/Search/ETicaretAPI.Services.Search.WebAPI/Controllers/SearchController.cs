using ETicaretAPI.Services.Search.Application.Services.SearchService;
using ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.Search.WebAPI.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Full-text product search with fuzziness and relevance scoring.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] ProductSearchFilterDto filter, CancellationToken cancellationToken = default)
    {
        var result = await _searchService.SearchAsync(filter, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Filter products by category, brand, price range, etc.
    /// </summary>
    [HttpGet("filter")]
    public async Task<IActionResult> Filter([FromQuery] ProductSearchFilterDto filter, CancellationToken cancellationToken = default)
    {
        var result = await _searchService.FilterAsync(filter, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Autocomplete suggestions for product names.
    /// </summary>
    [HttpGet("suggest")]
    public async Task<IActionResult> Suggest([FromQuery] string q, [FromQuery] int size = 5, CancellationToken cancellationToken = default)
    {
        var result = await _searchService.SuggestAsync(q, size, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Fetches all products from Catalog service and bulk-indexes them into Elasticsearch.
    /// </summary>
    [HttpPost("reindex")]
    public async Task<IActionResult> ReindexAll(CancellationToken cancellationToken = default)
    {
        var result = await _searchService.IndexAllProductsAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
