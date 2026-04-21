# Anti-Patterns — NEVER Do These

## Architecture Violations

### Business logic in controllers
```csharp
// WRONG
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
{
    var product = new Product { Name = dto.Name, Price = dto.Price };
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
    return Ok(product);
}

// CORRECT
[HttpPost("create")]
public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken = default)
{
    var result = await _productService.CreateAsync(dto, cancellationToken);
    return StatusCode(result.StatusCode, result);
}
```

### Direct DbContext access in controllers
- Controllers MUST only depend on `I*Service` interfaces
- Never inject `DbContext`, `IRepository`, or `IUnitOfWork` into controllers

### Cross-layer dependency violations
- Application layer referencing `Microsoft.EntityFrameworkCore`
- Domain layer referencing any project
- Services calling other services' repositories directly

---

## Data Access Anti-Patterns

### Fetching entire tables
```csharp
// WRONG
var allProducts = await _context.Products.ToListAsync();
return allProducts.Where(x => x.IsActive).Take(10);

// CORRECT
var products = await _productRepository.GetAllAsNoTrackingWithPaginationAsync(
    page, pageSize, predicate, orders, selector, cancellationToken);
```

### N+1 query problem
```csharp
// WRONG
var products = await _productRepository.GetAllAsync(x => x.IsActive);
foreach (var product in products)
{
    product.Category = await _categoryRepository.GetAsync(x => x.Id == product.CategoryId);
}

// CORRECT — use includes/selector
var selector = ProductSelector.GetProductWithDetailsSelector();
var products = await _productRepository.GetAllAsNoTrackingWithPaginationAsync(
    page, pageSize, predicate, orders, selector, cancellationToken);
```

### Tracked queries for read-only operations
```csharp
// WRONG
var product = await _productRepository.GetAsync(x => x.Id == id);

// CORRECT (read-only)
var product = await _productRepository.GetWithAsNoTrackingAsync(x => x.Id == id, includes, cancellationToken);
```

### Hard deleting entities
```csharp
// WRONG
_context.Products.Remove(product);

// CORRECT
await _productRepository.SoftDeleteAsync(product, cancellationToken);
```

---

## Service Anti-Patterns

### Returning raw entities from services
```csharp
// WRONG
public async Task<Product> GetByIdAsync(int id) { ... }

// CORRECT
public async Task<ApiResponse<GetProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default) { ... }
```

### Missing UnitOfWork save
```csharp
// WRONG — changes never persisted
await _productRepository.AddAsync(product, cancellationToken);
return ApiResponse<GetProductDto>.Success(result);

// CORRECT
await _productRepository.AddAsync(product, cancellationToken);
await _unitOfWork.SaveChangesAsync(cancellationToken);
return ApiResponse<GetProductDto>.Success(result);
```

### Forgetting cache invalidation
```csharp
// WRONG — stale cache after update
await _productRepository.UpdateAsync(product, cancellationToken);
await _unitOfWork.SaveChangesAsync(cancellationToken);

// CORRECT
await _productRepository.UpdateAsync(product, cancellationToken);
await _unitOfWork.SaveChangesAsync(cancellationToken);
await _cacheService.RemoveAsync(ProductCacheKey(dto.Id), cancellationToken);
```

---

## Async Anti-Patterns

### Sync-over-async
```csharp
// WRONG
var result = _productService.GetByIdAsync(id).Result;
var result = _productService.GetByIdAsync(id).GetAwaiter().GetResult();

// CORRECT
var result = await _productService.GetByIdAsync(id, cancellationToken);
```

### Fire-and-forget without error handling
```csharp
// WRONG
_ = _eventBus.PublishAsync(new OrderCreatedEvent { ... });

// CORRECT
await _eventBus.PublishAsync(new OrderCreatedEvent { ... });
```

### Missing CancellationToken propagation
```csharp
// WRONG
public async Task<ApiResponse<T>> DoWorkAsync()
{
    await _repo.GetAsync(x => x.Id == id);
}

// CORRECT
public async Task<ApiResponse<T>> DoWorkAsync(CancellationToken cancellationToken = default)
{
    await _repo.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);
}
```

---

## General Anti-Patterns

- **God services**: A single service doing everything. Split by aggregate/entity.
- **Anemic DTOs with public setters everywhere**: Use `init` for create DTOs.
- **Magic strings**: Use `nameof()`, constants, or enums.
- **Swallowing exceptions**: `catch (Exception) { }` is never acceptable.
- **Service locator**: `IServiceProvider.GetService<T>()` in business logic.
- **Circular dependencies**: Service A depends on Service B which depends on Service A.
- **Over-fetching**: Selecting all columns when you need three. Use Selectors.
- **Distributed transactions**: Do not attempt cross-service transactions. Use eventual consistency via events.
