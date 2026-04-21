# API Guidelines

## Route Convention

All routes follow the pattern: `api/{service}/[controller]/{action}`

```
api/catalog/products/getAllFilter
api/catalog/products/getById/{id:int}
api/catalog/products/create
api/catalog/products/update
api/catalog/products/delete/{id:int}
```

### DO
- Use `[ApiController]` and `[Route("api/{service}/[controller]")]` on every controller
- Use lowercase action names in routes
- Use route constraints (`{id:int}`) for type safety
- Use `[FromQuery]` for filter/pagination DTOs
- Use `[FromBody]` for create/update DTOs
- Use `[FromRoute]` (implicit with `[ApiController]`) for path parameters
- Accept `CancellationToken cancellationToken = default` in every action

### DO NOT
- Use RESTful-only routes (this project uses action-based routing)
- Return raw entity objects from controllers
- Use `Ok()`, `BadRequest()`, etc. — always use `StatusCode(result.StatusCode, result)`
- Put query logic, mapping, or validation in controllers
- Use `[Authorize]` without specifying a policy (when auth is implemented)

---

## Response Format

All API responses use `ApiResponse<T>`:

```json
// Success
{
    "isSuccess": true,
    "data": { ... },
    "message": "optional message",
    "errorCode": null,
    "errors": [],
    "statusCode": 200
}

// Failure
{
    "isSuccess": false,
    "data": null,
    "message": null,
    "errorCode": "PRODUCT_NOT_FOUND",
    "errors": ["Product with id 42 was not found"],
    "statusCode": 404
}
```

### Status Codes

| Operation | Success | Common Failures |
|---|---|---|
| GET (single) | 200 | 404 |
| GET (list) | 200 | 400 (invalid filter) |
| POST (create) | 200 | 400, 409 (duplicate) |
| PUT (update) | 200 | 400, 404 |
| DELETE | 200 | 404 |

---

## Controller Pattern

Every controller action follows this exact structure:

```csharp
[HttpGet("getById/{id:int}")]
public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
{
    var result = await _service.GetByIdAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
}
```

No try/catch in controllers. Exception handling is done at the middleware/service level.

---

## Pagination

All list endpoints accept filter DTOs extending `BaseFilterDto`:

```csharp
public class BaseFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; }
    public string? OrderType { get; set; } // "asc" or "desc"
}
```

Paginated responses use `PagedResult<T>` inside `ApiResponse<PagedResult<T>>`.

---

## Filter DTOs

Each entity's filter DTO inherits `BaseFilterDto` and adds domain-specific fields:

```csharp
public class GetProductForAdminFilterDto : BaseFilterDto
{
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsActive { get; set; }
}
```

All filter logic is handled by static `*Predicate.GetExpression(filterDto)` methods — never in the controller or service.

---

## CORS

Currently configured as `AllowAnyOrigin`. In production, restrict to specific frontend domains.

---

## Gateway Routing

All external traffic goes through the Ocelot API Gateway on port 5000:

```
GET  /api/catalog/products/getById/1  ->  localhost:5002/api/catalog/products/getById/1
POST /api/identity/auth/login         ->  localhost:5001/api/identity/auth/login
```

When adding a new controller or service, update `src/ApiGateway/ocelot.json` with the new route mapping.
