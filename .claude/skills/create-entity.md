# Skill: Create Entity

Create a complete entity with all supporting files across all layers.

---

## Input Required

- Service name (e.g., Catalog)
- Entity name (e.g., Review)
- Properties (name, type, nullable, constraints)
- Relationships (foreign keys, navigation properties)

---

## Files to Create (in order)

### 1. Entity

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Domain/Entities/{Entity}.cs`

```csharp
using ETicaretAPI.Common.Domain.Entities;

namespace ETicaretAPI.Services.{Service}.Domain.Entities;

public class {Entity} : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    // ... properties

    // Foreign keys
    public int? {Related}Id { get; set; }

    // Navigation properties
    public {Related}? {Related} { get; set; }
}
```

Rules:
- Always extend `Entity<int>` (inherits Id, IsDeleted, CreatedAt, ModifiedAt, DeletedAt, audit fields)
- Use nullable reference types for optional properties
- Initialize collection navigations: `ICollection<T> X { get; set; } = new List<T>();`
- Initialize strings with `string.Empty`

### 2. DTOs

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Domain/DTOs/{Entity}Dtos/`

**GetDto:**
```csharp
namespace ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

public class Get{Entity}Dto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... all readable properties
    public DateTime CreatedAt { get; set; }
}
```

**CreateDto:**
```csharp
public class Create{Entity}Dto
{
    public string Name { get; set; } = string.Empty;
    // ... writable properties (no Id, no audit fields)
}
```

**UpdateDto:**
```csharp
public class Update{Entity}Dto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... updatable properties
}
```

**FilterDto:**
```csharp
using ETicaretAPI.Common.Domain.DTOs;

public class Get{Entity}ForAdminFilterDto : BaseFilterDto
{
    public string? Name { get; set; }
    // ... filterable properties (all nullable)
}
```

### 3. Enum (OrderBy)

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Domain/Enums/{Entity}OrderByEnum.cs`

```csharp
namespace ETicaretAPI.Services.{Service}.Domain.Enums;

public enum {Entity}OrderByEnum
{
    Name,
    CreatedAt,
    // ... sortable fields
}
```

### 4. Repository Interface

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Repositories/I{Entity}Repository.cs`

```csharp
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.{Service}.Domain.Entities;

namespace ETicaretAPI.Services.{Service}.Application.Repositories;

public interface I{Entity}Repository : IEntityRepository<{Entity}>
{
}
```

### 5. Predicate

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Predicates/{Entity}Predicate.cs`

```csharp
using System.Linq.Expressions;
using ETicaretAPI.Common.Infrastructure.Helpers;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;
using ETicaretAPI.Services.{Service}.Domain.Entities;

namespace ETicaretAPI.Services.{Service}.Application.Predicates;

public static class {Entity}Predicate
{
    public static Expression<Func<{Entity}, bool>> GetExpression(Get{Entity}ForAdminFilterDto filter)
    {
        var predicate = PredicateBuilder.True<{Entity}>();

        predicate = predicate.And(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            predicate = predicate.And(x => x.Name!.Contains(filter.Name));

        // ... additional filters

        return predicate;
    }
}
```

### 6. Order

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Orders/{Entity}Order.cs`

```csharp
using System.Linq.Expressions;
using ETicaretAPI.Common.Domain.Enums;
using ETicaretAPI.Services.{Service}.Domain.Entities;
using ETicaretAPI.Services.{Service}.Domain.Enums;

namespace ETicaretAPI.Services.{Service}.Application.Orders;

public static class {Entity}Order
{
    public static (Expression<Func<{Entity}, object>> KeySelector, OrderTypeEnum OrderType) GetOrder(
        string? orderBy, string? orderType)
    {
        var orderEnum = Enum.TryParse<{Entity}OrderByEnum>(orderBy, true, out var parsed)
            ? parsed
            : {Entity}OrderByEnum.CreatedAt;

        var type = orderType?.ToLower() == "asc" ? OrderTypeEnum.Asc : OrderTypeEnum.Desc;

        Expression<Func<{Entity}, object>> keySelector = orderEnum switch
        {
            {Entity}OrderByEnum.Name => x => x.Name!,
            _ => x => x.CreatedAt
        };

        return (keySelector, type);
    }
}
```

### 7. AutoMapper Profile

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Mappings/AutoMapper/Profiles/{Entity}Profile.cs`

```csharp
using AutoMapper;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;
using ETicaretAPI.Services.{Service}.Domain.Entities;

namespace ETicaretAPI.Services.{Service}.Application.Mappings.AutoMapper.Profiles;

public class {Entity}Profile : Profile
{
    public {Entity}Profile()
    {
        CreateMap<{Entity}, Get{Entity}Dto>();
        CreateMap<Create{Entity}Dto, {Entity}>();
        CreateMap<Update{Entity}Dto, {Entity}>();
        CreateMap<IPagedResult<{Entity}>, PagedResult<Get{Entity}Dto>>();
    }
}
```

### 8. Service Interface

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Services/{Entity}Service/I{Entity}Service.cs`

```csharp
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;

namespace ETicaretAPI.Services.{Service}.Application.Services.{Entity}Service;

public interface I{Entity}Service
{
    Task<ApiResponse<PagedResult<Get{Entity}Dto>>> GetAllFilterAsync(Get{Entity}ForAdminFilterDto filterDto, CancellationToken cancellationToken = default);
    Task<ApiResponse<Get{Entity}Dto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<Get{Entity}Dto>> CreateAsync(Create{Entity}Dto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<Get{Entity}Dto>> UpdateAsync(Update{Entity}Dto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
```

### 9. Service Implementation (Manager)

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Application/Services/{Entity}Service/{Entity}Manager.cs`

```csharp
using AutoMapper;
using ETicaretAPI.Common.Application.Exceptions;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Application.Responses;
using ETicaretAPI.Common.Application.Results.Concrete;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Services.{Service}.Application.Orders;
using ETicaretAPI.Services.{Service}.Application.Predicates;
using ETicaretAPI.Services.{Service}.Application.Repositories;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;
using ETicaretAPI.Services.{Service}.Domain.Entities;

namespace ETicaretAPI.Services.{Service}.Application.Services.{Entity}Service;

public class {Entity}Manager : I{Entity}Service
{
    private readonly I{Entity}Repository _{entityCamel}Repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    private static string {Entity}CacheKey(int id) => $"{serviceLower}:{entityLower}:{id}";
    private static readonly TimeSpan {Entity}CacheTtl = TimeSpan.FromMinutes(60);

    public {Entity}Manager(
        I{Entity}Repository {entityCamel}Repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _{entityCamel}Repository = {entityCamel}Repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<PagedResult<Get{Entity}Dto>>> GetAllFilterAsync(
        Get{Entity}ForAdminFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var predicate = {Entity}Predicate.GetExpression(filterDto);
        var orders = {Entity}Order.GetOrder(filterDto.OrderBy, filterDto.OrderType);

        var entities = await _{entityCamel}Repository.GetAllAsNoTrackingWithPaginationAsync(
            filterDto.Page, filterDto.PageSize, predicate, orders, cancellationToken: cancellationToken);

        var result = _mapper.Map<PagedResult<Get{Entity}Dto>>(entities);
        return ApiResponse<PagedResult<Get{Entity}Dto>>.Success(result);
    }

    public async Task<ApiResponse<Get{Entity}Dto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cached = await _cacheService.GetAsync<Get{Entity}Dto>({Entity}CacheKey(id), cancellationToken);
        if (cached is not null)
            return ApiResponse<Get{Entity}Dto>.Success(cached);

        var entity = await _{entityCamel}Repository.GetWithAsNoTrackingAsync(
            x => x.Id == id, cancellationToken: cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof({Entity}), id);

        var result = _mapper.Map<Get{Entity}Dto>(entity);
        await _cacheService.SetAsync({Entity}CacheKey(id), result, {Entity}CacheTtl, cancellationToken);

        return ApiResponse<Get{Entity}Dto>.Success(result);
    }

    public async Task<ApiResponse<Get{Entity}Dto>> CreateAsync(Create{Entity}Dto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<{Entity}>(dto);

        await _{entityCamel}Repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<Get{Entity}Dto>(entity);
        await _cacheService.SetAsync({Entity}CacheKey(entity.Id), result, {Entity}CacheTtl, cancellationToken);

        return ApiResponse<Get{Entity}Dto>.Success(result);
    }

    public async Task<ApiResponse<Get{Entity}Dto>> UpdateAsync(Update{Entity}Dto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _{entityCamel}Repository.GetAsync(x => x.Id == dto.Id, cancellationToken: cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof({Entity}), dto.Id);

        _mapper.Map(dto, entity);

        await _{entityCamel}Repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync({Entity}CacheKey(dto.Id), cancellationToken);

        var result = _mapper.Map<Get{Entity}Dto>(entity);
        return ApiResponse<Get{Entity}Dto>.Success(result);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _{entityCamel}Repository.GetAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof({Entity}), id);

        await _{entityCamel}Repository.SoftDeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync({Entity}CacheKey(id), cancellationToken);

        return ApiResponse<bool>.Success(true);
    }
}
```

### 10. Repository Implementation

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Persistence/Repositories/{Entity}Repository.cs`

```csharp
using ETicaretAPI.Common.Persistence.Repositories;
using ETicaretAPI.Services.{Service}.Application.Repositories;
using ETicaretAPI.Services.{Service}.Domain.Entities;
using ETicaretAPI.Services.{Service}.Persistence.Context;

namespace ETicaretAPI.Services.{Service}.Persistence.Repositories;

public class {Entity}Repository : EfEntityRepositoryBase<{Entity}, {Service}DbContext>, I{Entity}Repository
{
    public {Entity}Repository({Service}DbContext context) : base(context)
    {
    }
}
```

### 11. EntityTypeConfiguration

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.Persistence/EntityTypeConfigurations/{Entity}Mapping.cs`

```csharp
using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.{Service}.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.{Service}.Persistence.EntityTypeConfigurations;

public class {Entity}Mapping : BaseEntityConfiguration<{Entity}>
{
    protected override void ConfigureEntity(EntityTypeBuilder<{Entity}> builder)
    {
        builder.ToTable("{table_name_snake_case_plural}");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        // ... configure all properties with snake_case column names

        // Indexes
        builder.HasIndex(x => x.Name);

        // Relationships
        builder.HasOne(x => x.{Related})
            .WithMany()
            .HasForeignKey(x => x.{Related}Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 12. Controller

File: `src/Services/{Service}/ETicaretAPI.Services.{Service}.WebAPI/Controllers/{Entity}sController.cs`

```csharp
using ETicaretAPI.Services.{Service}.Application.Services.{Entity}Service;
using ETicaretAPI.Services.{Service}.Domain.DTOs.{Entity}Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Services.{Service}.WebAPI.Controllers;

[ApiController]
[Route("api/{serviceLower}/[controller]")]
public class {Entity}sController : ControllerBase
{
    private readonly I{Entity}Service _{entityCamel}Service;

    public {Entity}sController(I{Entity}Service {entityCamel}Service)
    {
        _{entityCamel}Service = {entityCamel}Service;
    }

    [HttpGet("getAllFilter")]
    public async Task<IActionResult> GetAllFilter([FromQuery] Get{Entity}ForAdminFilterDto filterDto, CancellationToken cancellationToken = default)
    {
        var result = await _{entityCamel}Service.GetAllFilterAsync(filterDto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _{entityCamel}Service.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Create{Entity}Dto dto, CancellationToken cancellationToken = default)
    {
        var result = await _{entityCamel}Service.CreateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] Update{Entity}Dto dto, CancellationToken cancellationToken = default)
    {
        var result = await _{entityCamel}Service.UpdateAsync(dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _{entityCamel}Service.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
```

### 13. Register in DI

**ApplicationServiceRegistration.cs** — add:
```csharp
services.AddScoped<I{Entity}Service, {Entity}Manager>();
```

**PersistenceServiceRegistration.cs** — add:
```csharp
services.AddScoped<I{Entity}Repository, {Entity}Repository>();
```

**DbContext** — add:
```csharp
public DbSet<{Entity}> {Entity}s { get; set; }
```

### 14. Create Migration

```bash
dotnet ef migrations add Add{Entity}Table \
  --project src/Services/{Service}/ETicaretAPI.Services.{Service}.Persistence \
  --startup-project src/Services/{Service}/ETicaretAPI.Services.{Service}.WebAPI
```

---

## Post-Creation Checklist

- [ ] All 12 files created
- [ ] DI registrations added (Application + Persistence)
- [ ] DbSet added to DbContext
- [ ] EntityTypeConfiguration applied via BaseEntityConfiguration
- [ ] Migration generated and reviewed
- [ ] Ocelot routes updated if needed
- [ ] Build passes: `dotnet build ETicaretAPI.sln`
