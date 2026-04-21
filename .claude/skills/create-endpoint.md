# Skill: Create Endpoint

Add a new API endpoint to an existing service. This skill assumes the entity, service, and repository already exist.

---

## Input Required

- Service name (e.g., Catalog, Basket, Identity, Payment)
- Entity name (e.g., Product)
- Action name (e.g., GetById, Create, Update, Delete, GetAllFilter)
- HTTP method (GET, POST, PUT, DELETE)
- Input type (FromQuery, FromBody, FromRoute)

---

## Steps

### Step 1: Add method to service interface

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Services/{Entity}Service/I{Entity}Service.cs`

```csharp
Task<ApiResponse<{ReturnDto}>> {ActionName}Async({InputDto} dto, CancellationToken cancellationToken = default);
```

### Step 2: Implement in Manager

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Services/{Entity}Service/{Entity}Manager.cs`

```csharp
public async Task<ApiResponse<{ReturnDto}>> {ActionName}Async({InputDto} dto, CancellationToken cancellationToken = default)
{
    // 1. Validate/fetch entity
    // 2. Apply business logic
    // 3. Persist changes (if mutation)
    // 4. Invalidate cache (if mutation)
    // 5. Return ApiResponse<T>.Success(result)
}
```

### Step 3: Add controller action

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.WebAPI/Controllers/{Entity}sController.cs`

```csharp
[Http{Method}("{actionRoute}")]
public async Task<IActionResult> {ActionName}({InputBinding} {inputParam}, CancellationToken cancellationToken = default)
{
    var result = await _{entityCamel}Service.{ActionName}Async({inputParam}, cancellationToken);
    return StatusCode(result.StatusCode, result);
}
```

### Step 4: Update Ocelot gateway (if new route pattern)

File: `src/ApiGateway/ocelot.json`

Add route entry if the endpoint uses a new path pattern not already covered by existing wildcards.

---

## Checklist

- [ ] Service interface updated
- [ ] Manager implementation added with full error handling
- [ ] CancellationToken forwarded through entire chain
- [ ] Cache invalidated on mutations
- [ ] Controller action follows thin-controller pattern
- [ ] Route uses correct convention: `api/{service}/[controller]/{action}`
- [ ] Gateway updated if needed

---

## Example: Add "ToggleActive" endpoint to Products

**Interface:**
```csharp
Task<ApiResponse<bool>> ToggleActiveAsync(int id, CancellationToken cancellationToken = default);
```

**Manager:**
```csharp
public async Task<ApiResponse<bool>> ToggleActiveAsync(int id, CancellationToken cancellationToken = default)
{
    var product = await _productRepository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

    if (product is null)
        throw new NotFoundException(nameof(Product), id);

    product.IsActive = !product.IsActive;

    await _productRepository.UpdateFieldAsync(product, [x => x.IsActive], cancellationToken);
    await _cacheService.RemoveAsync(ProductCacheKey(id), cancellationToken);

    return ApiResponse<bool>.Success(product.IsActive);
}
```

**Controller:**
```csharp
[HttpPost("toggleActive/{id:int}")]
public async Task<IActionResult> ToggleActive(int id, CancellationToken cancellationToken = default)
{
    var result = await _productService.ToggleActiveAsync(id, cancellationToken);
    return StatusCode(result.StatusCode, result);
}
```
