# Skill: Create Elasticsearch Query

Add Elasticsearch-powered search to an entity in the Catalog service (or any service with ES configured).

---

## Input Required

- Service name (e.g., Catalog)
- Entity name (e.g., Product)
- Searchable fields (e.g., Name, Description, Code)
- Filter/facet fields (e.g., CategoryId, BrandId, Price range)
- Sort options

---

## Prerequisites

- Elasticsearch running on `localhost:9200`
- `NEST` or `Elastic.Clients.Elasticsearch` NuGet package installed
- `ISearchService` interface exists in `Common.Application.Interfaces`

---

## Steps

### Step 1: Define Search DTO

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Domain/DTOs/{Entity}Dtos/Search{Entity}Dto.cs`

```csharp
namespace ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

public class Search{Entity}Dto
{
    public string? Query { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}
```

### Step 2: Define ES Document Model

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Domain/DTOs/{Entity}Dtos/{Entity}SearchDocument.cs`

```csharp
namespace ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

public class {Entity}SearchDocument
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
```

### Step 3: Create Search Service Interface

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Services/{Entity}SearchService/I{Entity}SearchService.cs`

```csharp
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

namespace ETicaretAPI.Services.{Service}.Application.Services.{Entity}SearchService;

public interface I{Entity}SearchService
{
    Task<ApiResponse<PagedResult<{Entity}SearchDocument>>> SearchAsync(
        Search{Entity}Dto searchDto,
        CancellationToken cancellationToken = default);

    Task IndexDocumentAsync({Entity}SearchDocument document, CancellationToken cancellationToken = default);
    Task IndexBulkAsync(IEnumerable<{Entity}SearchDocument> documents, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(int id, CancellationToken cancellationToken = default);
}
```

### Step 4: Implement ES Search Service

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Infrastructure/Services/{Entity}SearchService/{Entity}ElasticsearchService.cs`

```csharp
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.{Service}.Application.Services.{Entity}SearchService;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

namespace ETicaretAPI.Services.{Service}.Infrastructure.Services.{Entity}SearchService;

public class {Entity}ElasticsearchService : I{Entity}SearchService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "{service_lower}_{entity_lower}s";

    public {Entity}ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<ApiResponse<PagedResult<{Entity}SearchDocument>>> SearchAsync(
        Search{Entity}Dto searchDto,
        CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();

        // Full-text search across multiple fields
        if (!string.IsNullOrWhiteSpace(searchDto.Query))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = searchDto.Query,
                Fields = new[] { "name^3", "description", "code^2" },
                Type = TextQueryType.BestFields,
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        // Filters (exact match, no scoring)
        if (searchDto.CategoryId.HasValue)
        {
            filterQueries.Add(new TermQuery("categoryId") { Value = searchDto.CategoryId.Value });
        }

        if (searchDto.BrandId.HasValue)
        {
            filterQueries.Add(new TermQuery("brandId") { Value = searchDto.BrandId.Value });
        }

        if (searchDto.MinPrice.HasValue || searchDto.MaxPrice.HasValue)
        {
            filterQueries.Add(new NumberRangeQuery("price")
            {
                Gte = searchDto.MinPrice.HasValue ? (double)searchDto.MinPrice.Value : null,
                Lte = searchDto.MaxPrice.HasValue ? (double)searchDto.MaxPrice.Value : null
            });
        }

        // Always filter active products
        filterQueries.Add(new TermQuery("isActive") { Value = true });

        var from = (searchDto.Page - 1) * searchDto.PageSize;

        var searchResponse = await _client.SearchAsync<{Entity}SearchDocument>(s => s
            .Index(IndexName)
            .From(from)
            .Size(searchDto.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries.ToArray())
                    .Filter(filterQueries.ToArray())
                )
            )
            .Sort(BuildSort(searchDto.SortBy, searchDto.SortOrder)),
            cancellationToken
        );

        if (!searchResponse.IsValidResponse)
        {
            return ApiResponse<PagedResult<{Entity}SearchDocument>>.Fail(
                "Elasticsearch query failed", 500);
        }

        var result = new PagedResult<{Entity}SearchDocument>
        {
            Items = searchResponse.Documents.ToList(),
            TotalCount = (int)searchResponse.Total,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };

        return ApiResponse<PagedResult<{Entity}SearchDocument>>.Success(result);
    }

    public async Task IndexDocumentAsync({Entity}SearchDocument document, CancellationToken cancellationToken = default)
    {
        await _client.IndexAsync(document, IndexName, document.Id.ToString(), cancellationToken);
    }

    public async Task IndexBulkAsync(IEnumerable<{Entity}SearchDocument> documents, CancellationToken cancellationToken = default)
    {
        await _client.BulkAsync(b => b
            .Index(IndexName)
            .IndexMany(documents),
            cancellationToken);
    }

    public async Task DeleteDocumentAsync(int id, CancellationToken cancellationToken = default)
    {
        await _client.DeleteAsync(IndexName, id.ToString(), cancellationToken);
    }

    private static Action<SortOptionsDescriptor<{Entity}SearchDocument>>[] BuildSort(string? sortBy, string? sortOrder)
    {
        var order = sortOrder?.ToLower() == "asc" ? SortOrder.Asc : SortOrder.Desc;

        return sortBy?.ToLower() switch
        {
            "price" => [s => s.Field(f => f.Price, d => d.Order(order))],
            "name" => [s => s.Field(f => f.Name, d => d.Order(order))],
            "createdat" => [s => s.Field(f => f.CreatedAt, d => d.Order(order))],
            _ => [s => s.Score(d => d.Order(SortOrder.Desc))]
        };
    }
}
```

### Step 5: Register in DI

In `{Service}ServiceRegistration.cs` (Infrastructure):

```csharp
// Elasticsearch client
var esUri = new Uri(configuration["Elasticsearch:Uri"] ?? "http://localhost:9200");
var esClient = new ElasticsearchClient(new ElasticsearchClientSettings(esUri));
services.AddSingleton(esClient);

services.AddScoped<I{Entity}SearchService, {Entity}ElasticsearchService>();
```

### Step 6: Integrate with Entity Manager

In `{Entity}Manager.CreateAsync` and `UpdateAsync`, after persisting to DB:

```csharp
// Index to Elasticsearch
var searchDoc = _mapper.Map<{Entity}SearchDocument>(entity);
await _{entityCamel}SearchService.IndexDocumentAsync(searchDoc, cancellationToken);
```

In `{Entity}Manager.DeleteAsync`:

```csharp
await _{entityCamel}SearchService.DeleteDocumentAsync(id, cancellationToken);
```

### Step 7: Add Search Controller Action

```csharp
[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] Search{Entity}Dto searchDto, CancellationToken cancellationToken = default)
{
    var result = await _{entityCamel}SearchService.SearchAsync(searchDto, cancellationToken);
    return StatusCode(result.StatusCode, result);
}
```

---

## Index Mapping (for reference)

Create index with proper mappings via Kibana or startup code:

```json
PUT /catalog_products
{
  "mappings": {
    "properties": {
      "id": { "type": "integer" },
      "name": { "type": "text", "fields": { "keyword": { "type": "keyword" } } },
      "description": { "type": "text" },
      "code": { "type": "keyword" },
      "price": { "type": "float" },
      "stockQuantity": { "type": "integer" },
      "isActive": { "type": "boolean" },
      "categoryId": { "type": "integer" },
      "categoryName": { "type": "text", "fields": { "keyword": { "type": "keyword" } } },
      "brandId": { "type": "integer" },
      "brandName": { "type": "text", "fields": { "keyword": { "type": "keyword" } } },
      "createdAt": { "type": "date" }
    }
  }
}
```

---

## Checklist

- [ ] Search DTO created
- [ ] ES document model created
- [ ] Search service interface in Application
- [ ] ES implementation in Infrastructure
- [ ] ElasticsearchClient registered in DI
- [ ] Entity Manager indexes on create/update, deletes on soft-delete
- [ ] Search endpoint added to controller
- [ ] ES index mapping applied
