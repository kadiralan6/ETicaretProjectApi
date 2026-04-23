using ETicaretAPI.Services.Search.Domain.Documents;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;

namespace ETicaretAPI.Services.Search.Infrastructure.Elasticsearch;

/// <summary>
/// Uygulama başlatıldığında Elasticsearch index'ini oluşturur veya günceller.
/// Mapping'ler: name (text + keyword + completion), description (text),
/// price (double), categoryName/brandName (text + keyword).
/// </summary>
public sealed class ElasticsearchIndexInitializer : IHostedService
{
    private readonly IElasticClient _client;
    private readonly ILogger<ElasticsearchIndexInitializer> _logger;
    private const string IndexName = "products";

    public ElasticsearchIndexInitializer(
        IElasticClient client,
        ILogger<ElasticsearchIndexInitializer> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existsResponse = await _client.Indices.ExistsAsync(IndexName, ct: cancellationToken);

            if (!existsResponse.IsValid)
            {
                _logger.LogWarning(
                    "Elasticsearch is not reachable (index check failed). " +
                    "Search service will start without index initialisation. " +
                    "Details: {Debug}", existsResponse.DebugInformation);
                return;
            }

            if (existsResponse.Exists)
            {
                _logger.LogInformation("Elasticsearch index '{IndexName}' already exists", IndexName);
                return;
            }

            _logger.LogInformation("Creating Elasticsearch index '{IndexName}'...", IndexName);

            var createResponse = await _client.Indices.CreateAsync(IndexName, c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Custom("turkish_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "turkish_stemmer", "asciifolding"))
                            .Custom("autocomplete_analyzer", ca => ca
                                .Tokenizer("autocomplete_tokenizer")
                                .Filters("lowercase", "asciifolding"))
                            .Custom("autocomplete_search_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "asciifolding")))
                        .TokenFilters(tf => tf
                            .Stemmer("turkish_stemmer", st => st.Language("turkish")))
                        .Tokenizers(t => t
                            .EdgeNGram("autocomplete_tokenizer", e => e
                                .MinGram(2)
                                .MaxGram(15)
                                .TokenChars(TokenChar.Letter, TokenChar.Digit)))))
                .Map<ProductDocument>(m => m
                    .Properties(p => p
                        .Number(n => n.Name(x => x.Id).Type(NumberType.Integer))
                        .Text(t => t
                            .Name(x => x.Code)
                            .Analyzer("standard")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))))
                        .Text(t => t
                            .Name(x => x.Name)
                            .Analyzer("turkish_analyzer")
                            .SearchAnalyzer("turkish_analyzer")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword").IgnoreAbove(256))
                                .Text(tx => tx
                                    .Name("autocomplete")
                                    .Analyzer("autocomplete_analyzer")
                                    .SearchAnalyzer("autocomplete_search_analyzer"))
                                .Completion(comp => comp.Name("suggest"))))
                        .Text(t => t
                            .Name(x => x.Description)
                            .Analyzer("turkish_analyzer")
                            .SearchAnalyzer("turkish_analyzer"))
                        .Keyword(k => k.Name(x => x.Slug))
                        .Number(n => n.Name(x => x.Price).Type(NumberType.Double))
                        .Number(n => n.Name(x => x.StockQuantity).Type(NumberType.Integer))
                        .Boolean(b => b.Name(x => x.IsActive))
                        .Boolean(b => b.Name(x => x.IsFeatured))
                        .Number(n => n.Name(x => x.CategoryId).Type(NumberType.Integer))
                        .Text(t => t
                            .Name(x => x.CategoryName)
                            .Analyzer("turkish_analyzer")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword").IgnoreAbove(256))))
                        .Number(n => n.Name(x => x.BrandId).Type(NumberType.Integer))
                        .Text(t => t
                            .Name(x => x.BrandName)
                            .Analyzer("turkish_analyzer")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword").IgnoreAbove(256))))
                        .Keyword(k => k.Name(x => x.ImageUrls))
                        .Date(d => d.Name(x => x.CreatedAt))
                        .Date(d => d.Name(x => x.ModifiedAt))
                    )), cancellationToken);

            if (!createResponse.IsValid)
            {
                var reason = createResponse.ServerError?.Error?.Reason
                             ?? createResponse.DebugInformation;
                _logger.LogError("Failed to create index. Details: {Debug}", createResponse.DebugInformation);
                throw new InvalidOperationException(
                    $"Failed to create Elasticsearch index '{IndexName}': {reason}");
            }

            _logger.LogInformation("Elasticsearch index '{IndexName}' created successfully", IndexName);
        }
        catch (InvalidOperationException)
        {
            // Re-throw mapping/creation errors as-is
            throw;
        }
        catch (Exception ex)
        {
            // Network-level errors (connection refused, timeout, etc.) — log and continue
            _logger.LogWarning(ex,
                "Elasticsearch is not reachable. Search service will start without index initialisation. " +
                "Start Elasticsearch and restart the service to enable search.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
