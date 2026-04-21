# Architecture Rules

## Layer Dependency Rules

### Domain Layer (`*.Domain`)
- Contains: Entities, DTOs, Enums, Value Objects
- Dependencies: `ETicaretAPI.Common.Domain` only
- **DO:** Define all data contracts (DTOs) here
- **DO:** Keep entities as pure data models with navigation properties
- **DO NOT:** Reference Application, Persistence, Infrastructure, or WebAPI
- **DO NOT:** Add business logic to entities
- **DO NOT:** Use any `Microsoft.EntityFrameworkCore` types here

### Application Layer (`*.Application`)
- Contains: Service interfaces (`I*Service`), Service implementations (`*Manager`), Repository interfaces, AutoMapper profiles, Predicates, Orders, Selectors
- Dependencies: Domain, `ETicaretAPI.Common.Application`, `ETicaretAPI.Common.Domain`
- **DO:** Define `I*Repository` interfaces here
- **DO:** Implement all business logic in `*Manager` classes
- **DO:** Return `ApiResponse<T>` from all public service methods
- **DO:** Use constructor injection for all dependencies
- **DO:** Forward `CancellationToken` to every async call
- **DO NOT:** Reference `Microsoft.EntityFrameworkCore` or `DbContext`
- **DO NOT:** Reference Persistence or Infrastructure projects
- **DO NOT:** Access `HttpContext` or any web-specific types

### Persistence Layer (`*.Persistence`)
- Contains: `DbContext`, Repository implementations, EntityTypeConfigurations, Migrations
- Dependencies: Application, Domain, `ETicaretAPI.Common.Persistence`
- **DO:** Extend `EfEntityRepositoryBase<TEntity, TContext>` for all repositories
- **DO:** Use `EntityTypeConfiguration` via `BaseEntityConfiguration<T>` for all mappings
- **DO:** Use snake_case for database column names
- **DO:** Configure indexes, constraints, and relationships in EntityTypeConfigurations
- **DO NOT:** Put business logic in repositories
- **DO NOT:** Call other services from repositories

### Infrastructure Layer (`*.Infrastructure`)
- Contains: Third-party service integrations (Cloudinary, Elasticsearch, external APIs)
- Dependencies: Application (for interfaces)
- **DO:** Implement Application-layer interfaces (e.g., `IImageUploadService`)
- **DO NOT:** Access `DbContext` or repositories directly

### WebAPI Layer (`*.WebAPI`)
- Contains: Controllers, `Program.cs`, `appsettings.json`
- Dependencies: Application, Infrastructure, Persistence (for DI registration only)
- **DO:** Keep controllers thin — single call to service, return `StatusCode(result.StatusCode, result)`
- **DO:** Use `[FromQuery]` for filter DTOs, `[FromBody]` for create/update DTOs
- **DO:** Accept `CancellationToken` in all action methods
- **DO NOT:** Put any business logic in controllers
- **DO NOT:** Call repositories directly from controllers
- **DO NOT:** Manipulate entities in controllers

---

## Cross-Service Communication

- **DO:** Use RabbitMQ integration events via `IEventBus.PublishAsync<T>()`
- **DO:** Define event contracts in `ETicaretAPI.Common.SharedLibrary.Events`
- **DO:** Use the exchange `eticaret_event_bus` (Topic type)
- **DO NOT:** Call another service's database directly
- **DO NOT:** Use synchronous HTTP calls for commands (queries only via `IRestService`)
- **DO NOT:** Duplicate entity definitions across services

---

## Shared Libraries Usage

| Library | Use For |
|---|---|
| `Common.Domain` | `Entity<T>`, `IEntity<T>`, `IUnitOfWork`, `BaseFilterDto` |
| `Common.Application` | `ApiResponse<T>`, `ICacheService`, `IEventBus`, `RedisCacheManager` |
| `Common.Persistence` | `EfEntityRepositoryBase`, generic repository implementation |
| `Common.Infrastructure` | `PredicateBuilder`, pagination/ordering extensions, `BaseEntityConfiguration<T>` |
| `Common.SharedLibrary` | Integration events, shared enums, `PaginationParams` |

---

## DI Registration Convention

Each layer exposes ONE extension method:
```csharp
// Persistence
public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)

// Application
public static IServiceCollection AddApplicationServices(this IServiceCollection services)

// Infrastructure (service-specific)
public static IServiceCollection Add{Service}Services(this IServiceCollection services, IConfiguration configuration)

// Common Infrastructure
public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
```

`Program.cs` calls them in order: **Persistence -> Application -> Infrastructure -> CommonInfrastructure**.
