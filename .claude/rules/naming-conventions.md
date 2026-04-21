# Naming Conventions

## Project Naming

```
ETicaretAPI.Services.{ServiceName}.{Layer}
ETicaretAPI.Common.{LibraryName}
```

Examples:
- `ETicaretAPI.Services.Catalog.Domain`
- `ETicaretAPI.Services.Catalog.Application`
- `ETicaretAPI.Services.Catalog.Persistence`
- `ETicaretAPI.Services.Catalog.Infrastructure`
- `ETicaretAPI.Services.Catalog.WebAPI`

---

## Namespace Conventions

```csharp
namespace ETicaretAPI.Services.Catalog.Domain.Entities;
namespace ETicaretAPI.Services.Catalog.Application.Services.ProductService;
namespace ETicaretAPI.Services.Catalog.Persistence.Repositories;
namespace ETicaretAPI.Services.Catalog.WebAPI.Controllers;
```

---

## File & Class Naming

| Type | Pattern | Example |
|---|---|---|
| Entity | `{Name}` | `Product`, `Brand`, `Category` |
| Get DTO | `Get{Entity}Dto` | `GetProductDto` |
| Create DTO | `Create{Entity}Dto` | `CreateProductDto` |
| Update DTO | `Update{Entity}Dto` | `UpdateProductDto` |
| Filter DTO | `Get{Entity}ForAdminFilterDto` | `GetProductForAdminFilterDto` |
| Service interface | `I{Entity}Service` | `IProductService` |
| Service implementation | `{Entity}Manager` | `ProductManager` |
| Repository interface | `I{Entity}Repository` | `IProductRepository` |
| Repository implementation | `{Entity}Repository` | `ProductRepository` |
| Controller | `{Entity}sController` (plural) | `ProductsController` |
| AutoMapper profile | `{Entity}Profile` | `ProductProfile` |
| EF configuration | `{Entity}Mapping` | `ProductMapping` |
| Predicate builder | `{Entity}Predicate` | `ProductPredicate` |
| Order builder | `{Entity}Order` | `ProductOrder` |
| Selector builder | `{Entity}Selector` | `ProductSelector` |
| Enum | `{Entity}{Property}Enum` | `ProductOrderByEnum` |
| Integration event | `{Action}{Past}Event` | `OrderCreatedEvent`, `PaymentCompletedEvent` |
| DI registration | `{Layer}ServiceRegistration` | `PersistenceServiceRegistration` |

---

## Directory Structure per Entity

```
Domain/
  Entities/{Entity}.cs
  DTOs/{Entity}Dtos/
    Get{Entity}Dto.cs
    Create{Entity}Dto.cs
    Update{Entity}Dto.cs
    Get{Entity}ForAdminFilterDto.cs
  Enums/{Entity}OrderByEnum.cs

Application/
  Services/{Entity}Service/
    I{Entity}Service.cs
    {Entity}Manager.cs
  Repositories/I{Entity}Repository.cs
  Mappings/AutoMapper/Profiles/{Entity}Profile.cs
  Predicates/{Entity}Predicate.cs
  Orders/{Entity}Order.cs
  Selectors/{Entity}Selector.cs

Persistence/
  Repositories/{Entity}Repository.cs
  EntityTypeConfigurations/{Entity}Mapping.cs

WebAPI/
  Controllers/{Entity}sController.cs
```

---

## Database Naming (PostgreSQL)

| Type | Convention | Example |
|---|---|---|
| Table name | snake_case plural | `products`, `product_images` |
| Column name | snake_case | `stock_quantity`, `is_active` |
| Primary key | `id` | `id` |
| Foreign key column | `{referenced_table_singular}_id` | `category_id`, `brand_id` |
| Index name | `ix_{table}_{column}` | `ix_products_slug` |
| Boolean column | `is_{adjective}` | `is_active`, `is_deleted` |
| Timestamp column | `{action}_at` | `created_at`, `modified_at` |

---

## Variable & Method Naming

| Type | Convention | Example |
|---|---|---|
| Private field | `_camelCase` | `_productRepository` |
| Local variable | `camelCase` | `product`, `filterDto` |
| Method | `PascalCaseAsync` | `GetByIdAsync`, `CreateAsync` |
| Constant | `PascalCase` | `ProductCacheTtl` |
| Static method | `PascalCase` | `ProductCacheKey(int id)` |
| Parameter | `camelCase` | `cancellationToken` |
| Generic type | `T{Name}` | `TPrimaryKey`, `TEntity` |

---

## Cache Key Naming

Format: `{service}:{entity}:{identifier}`

```csharp
private static string ProductCacheKey(int id) => $"catalog:product:{id}";
private static string BrandCacheKey(int id) => $"catalog:brand:{id}";
private static string CategoryCacheKey(int id) => $"catalog:category:{id}";
```

---

## RabbitMQ Naming

- Exchange: `eticaret_event_bus` (Topic type)
- Queue: `{EventName}_queue` (e.g., `OrderCreatedEvent_queue`)
- Routing key: event class name
