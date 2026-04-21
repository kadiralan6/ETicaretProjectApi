# Coding Standards

## General C# Standards

### DO
- Use file-scoped namespaces (`namespace X;`)
- Use `var` when the type is obvious from the right side
- Use pattern matching (`is not null`, `is { }`) over null checks
- Use collection expressions (`[]`) where supported in .NET 8
- Use `string.Empty` instead of `""`
- Use `nameof()` for exception messages and parameter references
- Use `readonly` for fields that don't change after construction
- Use `sealed` on classes that are not designed for inheritance
- Mark all async methods with `Async` suffix
- Always pass and forward `CancellationToken` in async chains
- Use `ConfigureAwait(false)` in library code (Common projects)

### DO NOT
- Use `#region` blocks
- Use `this.` qualifier unless resolving ambiguity
- Use `public` fields — always use properties
- Use `dynamic` type
- Use `Thread.Sleep` — use `Task.Delay` with cancellation
- Use synchronous I/O (`Wait()`, `.Result`) in async code
- Throw `Exception` directly — use specific exception types
- Catch `Exception` unless at the top-level middleware

---

## Async/Await Best Practices

```csharp
// CORRECT: Forward CancellationToken everywhere
public async Task<ApiResponse<GetProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
{
    var product = await _productRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);
    // ...
}

// WRONG: Missing CancellationToken
public async Task<ApiResponse<GetProductDto>> GetByIdAsync(int id)
{
    var product = await _productRepository.GetAsync(x => x.Id == id);
}
```

---

## Dependency Injection

- Use constructor injection exclusively
- Register services with appropriate lifetimes:
  - `Scoped`: Repositories, DbContext, UnitOfWork, Services (Managers)
  - `Singleton`: Cache clients, RabbitMQ connection, configuration objects
  - `Transient`: Stateless utilities only
- Never use `IServiceProvider.GetService<T>()` inside business logic
- Never use `new` to create service instances

---

## Entity Framework Core

- Always use `AsNoTracking()` for read-only queries
- Use `GetAllAsNoTrackingWithPaginationAsync` for list endpoints
- Use `GetWithAsNoTrackingAsync` for single-entity reads
- Use `GetAsync` (tracked) only when you need to update
- Use `UpdateFieldAsync` for partial updates (e.g., stock quantity)
- Use `SoftDeleteAsync` — never hard-delete domain entities
- Always call `_unitOfWork.SaveChangesAsync()` after mutations
- Define all relationships and indexes in `EntityTypeConfiguration`

---

## Error Handling

- Throw `NotFoundException` when entity lookup returns null
- Throw `ValidationException` for business rule violations
- Let the global exception middleware handle response formatting
- Use `ApiResponse<T>.Fail()` for expected business errors
- Use `ApiResponse<T>.Success()` for all successful operations
- Always include meaningful error messages

---

## Caching Strategy

- Use `ICacheService` (Redis) for hot-path data
- Cache key format: `{service}:{entity}:{id}` (e.g., `catalog:product:42`)
- Default TTL: 30-60 minutes for entity lookups
- Invalidate cache on create, update, and delete operations
- Use `GetOrAddAsync` for cache-aside pattern when appropriate
- Never cache paginated results (they change too frequently)

---

## Mapping (AutoMapper)

- One Profile per entity, placed in `Application/Mappings/AutoMapper/Profiles/`
- Map entity -> GetDto, CreateDto -> entity, UpdateDto -> entity
- Never map directly between entities of different services
- Use `.ForMember()` for non-trivial mappings
- Profile naming: `{Entity}Profile` (e.g., `ProductProfile`)
