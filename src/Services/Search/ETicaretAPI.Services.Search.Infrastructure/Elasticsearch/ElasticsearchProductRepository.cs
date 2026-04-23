using ETicaretAPI.Services.Search.Application.Repositories;
using ETicaretAPI.Services.Search.Domain.Documents;
using ETicaretAPI.Services.Search.Domain.DTOs.SearchDtos;
using Microsoft.Extensions.Logging;
using Nest;
using Polly;
using Polly.Retry;

namespace ETicaretAPI.Services.Search.Infrastructure.Elasticsearch;

public sealed class ElasticsearchProductRepository : IProductSearchRepository
{
    private readonly IElasticClient _client;
    private readonly ILogger<ElasticsearchProductRepository> _logger;
    private const string IndexName = "products";

    private readonly AsyncRetryPolicy _retryPolicy;

    public ElasticsearchProductRepository(
        IElasticClient client,
        ILogger<ElasticsearchProductRepository> logger)
    {
        _client = client;
        _logger = logger;

        _retryPolicy = Polly.Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Elasticsearch retry {RetryCount} after {Delay}s",
                        retryCount, timeSpan.TotalSeconds);
                });
    }

    public async Task IndexProductAsync(ProductDocument document, CancellationToken cancellationToken = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.IndexAsync(document, i => i
                .Index(IndexName)
                .Id(document.Id)
                .Refresh(global::Elasticsearch.Net.Refresh.WaitFor), cancellationToken);

            if (!response.IsValid)
            {
                _logger.LogError("Failed to index product {ProductId}: {Error}",
                    document.Id, response.DebugInformation);
                throw new InvalidOperationException(
                    $"Failed to index product {document.Id}: {response.ServerError?.Error?.Reason}");
            }
        });
    }

    public async Task UpdateProductAsync(ProductDocument document, CancellationToken cancellationToken = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.UpdateAsync<ProductDocument>(document.Id, u => u
                .Index(IndexName)
                .Doc(document)
                .DocAsUpsert()
                .Refresh(global::Elasticsearch.Net.Refresh.WaitFor), cancellationToken);

            if (!response.IsValid)
            {
                _logger.LogError("Failed to update product {ProductId}: {Error}",
                    document.Id, response.DebugInformation);
                throw new InvalidOperationException(
                    $"Failed to update product {document.Id}: {response.ServerError?.Error?.Reason}");
            }
        });
    }

    public async Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.DeleteAsync<ProductDocument>(productId, d => d
                .Index(IndexName)
                .Refresh(global::Elasticsearch.Net.Refresh.WaitFor), cancellationToken);

            if (!response.IsValid && response.Result != Result.NotFound)
            {
                _logger.LogError("Failed to delete product {ProductId}: {Error}",
                    productId, response.DebugInformation);
                throw new InvalidOperationException(
                    $"Failed to delete product {productId}: {response.ServerError?.Error?.Reason}");
            }
        });
    }

    public async Task<SearchPagedResultDto<ProductSearchResultDto>> SearchAsync(
        ProductSearchFilterDto filter, CancellationToken cancellationToken = default)
    {
        var from = (filter.Page - 1) * filter.PageSize;

        var response = await _client.SearchAsync<ProductDocument>(s =>
        {
            s.Index(IndexName)
             .From(from)
             .Size(filter.PageSize)
             .TrackTotalHits();

            // Build query
            s.Query(q => BuildQuery(q, filter));

            // Build sort
            s.Sort(sort => BuildSort(sort, filter));

            // Highlight matching fields
            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                s.Highlight(h => h
                    .Fields(
                        f => f.Field(p => p.Name).PreTags("<em>").PostTags("</em>"),
                        f => f.Field(p => p.Description).PreTags("<em>").PostTags("</em>")
                    ));
            }

            return s;
        }, cancellationToken);

        if (!response.IsValid)
        {
            _logger.LogError("Search failed: {Error}", response.DebugInformation);
            return new SearchPagedResultDto<ProductSearchResultDto>
            {
                CurrentPage = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = 0
            };
        }

        var results = response.Hits.Select(hit => new ProductSearchResultDto
        {
            Id = hit.Source.Id,
            Code = hit.Source.Code,
            Name = hit.Source.Name,
            Description = hit.Source.Description,
            Slug = hit.Source.Slug,
            Price = hit.Source.Price,
            StockQuantity = hit.Source.StockQuantity,
            IsActive = hit.Source.IsActive,
            IsFeatured = hit.Source.IsFeatured,
            CategoryName = hit.Source.CategoryName,
            BrandName = hit.Source.BrandName,
            ImageUrls = hit.Source.ImageUrls,
            Score = hit.Score ?? 0
        }).ToList();

        return new SearchPagedResultDto<ProductSearchResultDto>
        {
            Results = results,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = response.Total
        };
    }

    public async Task<List<SearchSuggestionDto>> SuggestAsync(
        string query, int size = 5, CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAsync<ProductDocument>(s => s
            .Index(IndexName)
            .Size(0)
            .Suggest(su => su
                .Completion("product-suggest", c => c
                    .Field(f => f.Name.Suffix("suggest"))
                    .Prefix(query)
                    .Fuzzy(f => f
                        .Fuzziness(Fuzziness.Auto))
                    .Size(size)
                )
            ), cancellationToken);

        if (!response.IsValid)
        {
            _logger.LogError("Suggest failed: {Error}", response.DebugInformation);

            // Fallback: use prefix query on name
            return await SuggestFallbackAsync(query, size, cancellationToken);
        }

        var suggestions = response.Suggest["product-suggest"]
            .SelectMany(s => s.Options)
            .Select(o => new SearchSuggestionDto
            {
                Text = o.Text,
                Score = o.Score
            })
            .ToList();

        if (suggestions.Count == 0)
            return await SuggestFallbackAsync(query, size, cancellationToken);

        return suggestions;
    }

    public async Task<bool> ExistsAsync(int productId, CancellationToken cancellationToken = default)
    {
        var response = await _client.DocumentExistsAsync<ProductDocument>(productId,
            d => d.Index(IndexName), cancellationToken);
        return response.Exists;
    }

    public async Task BulkIndexAsync(IEnumerable<ProductDocument> documents, CancellationToken cancellationToken = default)
    {
        var bulkResponse = await _client.BulkAsync(b => b
            .Index(IndexName)
            .IndexMany(documents)
            .Refresh(global::Elasticsearch.Net.Refresh.WaitFor), cancellationToken);

        if (bulkResponse.Errors)
        {
            var failedIds = bulkResponse.ItemsWithErrors.Select(i => i.Id);
            _logger.LogError("Bulk index failed for documents: {FailedIds}", string.Join(", ", failedIds));
            throw new InvalidOperationException($"Bulk index had {bulkResponse.ItemsWithErrors.Count()} failures");
        }

        _logger.LogInformation("Bulk indexed {Count} products", bulkResponse.Items.Count);
    }

    private static QueryContainer BuildQuery(QueryContainerDescriptor<ProductDocument> q, ProductSearchFilterDto filter)
    {
        var mustQueries = new List<Func<QueryContainerDescriptor<ProductDocument>, QueryContainer>>();
        var filterQueries = new List<Func<QueryContainerDescriptor<ProductDocument>, QueryContainer>>();

        // Full-text search with fuzziness and boosting
        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            mustQueries.Add(mq => mq
                .MultiMatch(mm => mm
                    .Fields(f => f
                        .Field(p => p.Name, boost: 3)
                        .Field(p => p.Description, boost: 1)
                        .Field(p => p.BrandName, boost: 2)
                        .Field(p => p.CategoryName, boost: 2)
                        .Field(p => p.Code, boost: 1.5))
                    .Query(filter.Query)
                    .Fuzziness(Fuzziness.Auto)
                    .Type(TextQueryType.BestFields)
                    .Operator(Operator.Or)
                    .MinimumShouldMatch("75%")
                ));
        }

        // Category filter (keyword — exact match)
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            filterQueries.Add(fq => fq
                .Term(t => t.Field(p => p.CategoryName.Suffix("keyword")).Value(filter.Category)));
        }

        // Brand filter (keyword — exact match)
        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            filterQueries.Add(fq => fq
                .Term(t => t.Field(p => p.BrandName.Suffix("keyword")).Value(filter.Brand)));
        }

        // Price range filter
        if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
        {
            filterQueries.Add(fq => fq
                .Range(r =>
                {
                    var rangeQuery = r.Field(p => p.Price);
                    if (filter.MinPrice.HasValue)
                        rangeQuery = rangeQuery.GreaterThanOrEquals((double)filter.MinPrice.Value);
                    if (filter.MaxPrice.HasValue)
                        rangeQuery = rangeQuery.LessThanOrEquals((double)filter.MaxPrice.Value);
                    return rangeQuery;
                }));
        }

        // Active filter
        if (filter.IsActive.HasValue)
        {
            filterQueries.Add(fq => fq
                .Term(t => t.Field(p => p.IsActive).Value(filter.IsActive.Value)));
        }

        // Featured filter
        if (filter.IsFeatured.HasValue)
        {
            filterQueries.Add(fq => fq
                .Term(t => t.Field(p => p.IsFeatured).Value(filter.IsFeatured.Value)));
        }

        // In-stock filter
        if (filter.InStock.HasValue && filter.InStock.Value)
        {
            filterQueries.Add(fq => fq
                .Range(r => r.Field(p => p.StockQuantity).GreaterThan(0)));
        }

        // If no must queries, use match_all
        if (mustQueries.Count == 0)
        {
            mustQueries.Add(mq => mq.MatchAll());
        }

        return q.Bool(b => b
            .Must(mustQueries.ToArray())
            .Filter(filterQueries.ToArray()));
    }

    private static IPromise<IList<ISort>> BuildSort(
        SortDescriptor<ProductDocument> sort, ProductSearchFilterDto filter)
    {
        var isDesc = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return filter.SortBy?.ToLowerInvariant() switch
        {
            "price" => sort.Field(f => f.Field(p => p.Price).Order(isDesc ? SortOrder.Descending : SortOrder.Ascending)),
            "name" => sort.Field(f => f.Field(p => p.Name.Suffix("keyword")).Order(isDesc ? SortOrder.Descending : SortOrder.Ascending)),
            "date" => sort.Field(f => f.Field(p => p.CreatedAt).Order(isDesc ? SortOrder.Descending : SortOrder.Ascending)),
            "stock" => sort.Field(f => f.Field(p => p.StockQuantity).Order(isDesc ? SortOrder.Descending : SortOrder.Ascending)),
            _ => sort.Field(f => f.Field("_score").Order(SortOrder.Descending))
        };
    }

    private async Task<List<SearchSuggestionDto>> SuggestFallbackAsync(
        string query, int size, CancellationToken cancellationToken)
    {
        var response = await _client.SearchAsync<ProductDocument>(s => s
            .Index(IndexName)
            .Size(size)
            .Source(src => src.Includes(i => i.Field(f => f.Name)))
            .Query(q => q
                .Prefix(p => p
                    .Field(f => f.Name.Suffix("keyword"))
                    .Value(query.ToLowerInvariant())
                    .CaseInsensitive()
                )), cancellationToken);

        if (!response.IsValid)
            return new List<SearchSuggestionDto>();

        return response.Hits
            .Where(h => h.Source.Name is not null)
            .Select(h => new SearchSuggestionDto
            {
                Text = h.Source.Name!,
                Score = h.Score ?? 0
            })
            .DistinctBy(s => s.Text)
            .ToList();
    }
}
