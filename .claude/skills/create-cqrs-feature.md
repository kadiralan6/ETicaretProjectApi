# Skill: Create CQRS Feature

Create a complete feature following the Command/Query pattern used in this project. While we don't use MediatR yet, the service layer follows CQRS principles: queries are read-only with `AsNoTracking`, commands mutate state via `UnitOfWork`.

---

## Input Required

- Service name (e.g., Catalog)
- Entity name (e.g., Product)
- Feature type: Query (read) or Command (write)
- Feature name (e.g., GetProductsByCategory, DeactivateExpiredProducts)

---

## Query Feature (Read Operation)

### Step 1: Create Filter DTO (if paginated)

File: `src/Services/{Service}/.../Domain/DTOs/{Entity}Dtos/Get{Entity}By{Criteria}FilterDto.cs`

```csharp
using ETicaretAPI.Common.Domain.DTOs;

namespace ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

public class Get{Entity}By{Criteria}FilterDto : BaseFilterDto
{
    public {FilterType}? {FilterProperty} { get; set; }
}
```

### Step 2: Create Predicate

File: `src/Services/{Service}/.../Application/Predicates/{Entity}Predicate.cs`

Add a new static method or extend the existing `GetExpression`:

```csharp
public static Expression<Func<{Entity}, bool>> GetBy{Criteria}Expression(Get{Entity}By{Criteria}FilterDto filter)
{
    var predicate = PredicateBuilder.True<{Entity}>();

    predicate = predicate.And(x => !x.IsDeleted);

    if (filter.{FilterProperty}.HasValue)
        predicate = predicate.And(x => x.{Property} == filter.{FilterProperty}.Value);

    return predicate;
}
```

### Step 3: Create Selector (if projection needed)

File: `src/Services/{Service}/.../Application/Selectors/{Entity}Selector.cs`

```csharp
public static Expression<Func<{Entity}, {Entity}>> Get{Entity}With{Details}Selector()
{
    return x => new {Entity}
    {
        Id = x.Id,
        Name = x.Name,
        // ... select only needed fields and navigations
    };
}
```

### Step 4: Add to service interface and Manager

```csharp
// Interface
Task<ApiResponse<PagedResult<Get{Entity}Dto>>> Get{Feature}Async(
    Get{Entity}By{Criteria}FilterDto filterDto,
    CancellationToken cancellationToken = default);

// Manager
public async Task<ApiResponse<PagedResult<Get{Entity}Dto>>> Get{Feature}Async(
    Get{Entity}By{Criteria}FilterDto filterDto,
    CancellationToken cancellationToken = default)
{
    var predicate = {Entity}Predicate.GetBy{Criteria}Expression(filterDto);
    var orders = {Entity}Order.GetOrder(filterDto.OrderBy, filterDto.OrderType);
    var selector = {Entity}Selector.Get{Entity}With{Details}Selector();

    var entities = await _{entityCamel}Repository.GetAllAsNoTrackingWithPaginationAsync(
        filterDto.Page, filterDto.PageSize, predicate, orders, selector, cancellationToken);

    var result = _mapper.Map<PagedResult<Get{Entity}Dto>>(entities);
    return ApiResponse<PagedResult<Get{Entity}Dto>>.Success(result);
}
```

---

## Command Feature (Write Operation)

### Step 1: Create Command DTO

File: `src/Services/{Service}/.../Domain/DTOs/{Entity}Dtos/{Action}{Entity}Dto.cs`

```csharp
namespace ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

public class {Action}{Entity}Dto
{
    public int {Entity}Id { get; set; }
    // ... command-specific properties
}
```

### Step 2: Implement in Manager

```csharp
public async Task<ApiResponse<{ReturnType}>> {Action}Async(
    {Action}{Entity}Dto dto,
    CancellationToken cancellationToken = default)
{
    // 1. Load entity (tracked — we need to modify it)
    var entity = await _{entityCamel}Repository.GetAsync(
        x => x.Id == dto.{Entity}Id,
        cancellationToken: cancellationToken);

    if (entity is null)
        throw new NotFoundException(nameof({Entity}), dto.{Entity}Id);

    // 2. Apply business logic
    entity.{Property} = dto.{NewValue};

    // 3. Persist
    await _{entityCamel}Repository.UpdateAsync(entity, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // 4. Invalidate cache
    await _cacheService.RemoveAsync({Entity}CacheKey(dto.{Entity}Id), cancellationToken);

    // 5. Publish event if cross-service impact
    await _eventBus.PublishAsync(new {Entity}{Action}Event
    {
        {Entity}Id = entity.Id,
        // ...
    });

    // 6. Return
    var result = _mapper.Map<Get{Entity}Dto>(entity);
    return ApiResponse<Get{Entity}Dto>.Success(result);
}
```

### Step 3: Add controller action and gateway route

Follow the `create-endpoint.md` skill for this step.

---

## Checklist

- [ ] DTO created in Domain layer
- [ ] Predicate/Selector created or updated (queries only)
- [ ] Service interface method added
- [ ] Manager implementation with full error handling
- [ ] Cache strategy: read-through for queries, invalidate for commands
- [ ] Events published for cross-service side effects (commands only)
- [ ] Controller action added
- [ ] UnitOfWork.SaveChangesAsync called after all mutations
