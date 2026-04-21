# System Architecture

## Overview

ETicaretAPI is a .NET 8 microservices-based e-commerce backend consisting of 4 domain services behind an Ocelot API Gateway. Each service owns its database and communicates asynchronously via RabbitMQ integration events.

```
                         ┌──────────────┐
                         │   Frontend   │
                         └──────┬───────┘
                                │
                         ┌──────▼───────┐
                         │  API Gateway │ :5000
                         │   (Ocelot)   │
                         └──────┬───────┘
                                │
        ┌───────────┬───────────┼───────────┬───────────┐
        │           │           │           │           │
  ┌─────▼─────┐ ┌──▼──────┐ ┌──▼──────┐ ┌──▼──────┐   │
  │ Identity  │ │ Catalog │ │ Basket  │ │ Payment │   │
  │  :5001    │ │  :5002  │ │  :5003  │ │  :5004  │   │
  └─────┬─────┘ └──┬──┬──┘ └──┬──────┘ └──┬──────┘   │
        │          │  │        │           │           │
        ▼          ▼  ▼        ▼           ▼           │
   PostgreSQL  PgSQL  ES    PostgreSQL  PostgreSQL     │
                      │                                │
                      │     ┌──────────┐               │
                      └────►│  Redis   │◄──────────────┘
                            └──────────┘
                            ┌──────────┐
                            │ RabbitMQ │ (async messaging)
                            └──────────┘
```

---

## Clean Architecture (per service)

```
┌─────────────────────────────────────────────┐
│                  WebAPI                       │
│  Controllers, Program.cs, appsettings        │
├─────────────────────────────────────────────┤
│              Infrastructure                   │
│  Third-party integrations (Cloudinary, ES)    │
├─────────────────────────────────────────────┤
│               Application                     │
│  Services (*Manager), Repositories (I*),      │
│  AutoMapper, Predicates, Orders, Selectors    │
├─────────────────────────────────────────────┤
│                 Domain                        │
│  Entities, DTOs, Enums, Value Objects         │
└─────────────────────────────────────────────┘
```

**Dependency direction: outer layers depend on inner layers. Never the reverse.**

---

## Common Libraries

Shared code lives in `src/Common/` and is referenced by all services:

| Library | Purpose |
|---|---|
| `Common.Domain` | Base entity, interfaces (`IEntity<T>`, `IUnitOfWork`, `IPagedResult`), `BaseFilterDto` |
| `Common.Application` | `ApiResponse<T>`, `ICacheService`, `IEventBus`, `IRestService`, exception types |
| `Common.Persistence` | `EfEntityRepositoryBase<TEntity, TContext>` — generic EF Core repository |
| `Common.Infrastructure` | `PredicateBuilder`, pagination/ordering extensions, `BaseEntityConfiguration<T>` |
| `Common.SharedLibrary` | Integration event contracts, shared enums, `PaginationParams` |

---

## Database Strategy

- Each service has its own PostgreSQL database (database-per-service pattern)
- No cross-service joins or foreign keys
- EF Core with Code-First migrations
- All tables use snake_case naming
- Soft delete via `is_deleted` / `deleted_at` columns (inherited from `Entity<T>`)
- Audit fields: `created_at`, `created_by`, `modified_at`, `modified_by`

---

## Caching Layer

- Redis (`localhost:6379`) via `IDistributedCache` abstraction
- `RedisCacheManager` implements `ICacheService`
- Cache-aside pattern: check cache -> miss -> load from DB -> populate cache
- Key format: `{service}:{entity}:{id}`
- Invalidation: explicit removal on create/update/delete
- Default TTL: 30-60 minutes

---

## API Gateway

- Ocelot-based gateway on port 5000
- Routes all external traffic to downstream services
- Pattern: `/api/{service}/**` -> `localhost:{port}/api/{service}/**`
- No authentication/authorization at gateway level yet
- Configuration in `src/ApiGateway/ocelot.json`

---

## Authentication (Identity Service)

- JWT-based authentication managed by the Identity service
- `AppUser` extends ASP.NET Core Identity's `IdentityUser`
- Token generation via `TokenManager`
- Refresh token support
- Role-based authorization (`AppRole`)

---

## Key Design Decisions

1. **Service pattern over CQRS mediator**: Business logic lives in `*Manager` classes, not MediatR handlers. Services return `ApiResponse<T>` directly.
2. **Predicate/Order/Selector separation**: Filtering, sorting, and projection are encapsulated in dedicated static classes rather than inline in services.
3. **Generic repository**: `EfEntityRepositoryBase` provides full CRUD with soft delete and partial update support. Service-specific repositories extend it.
4. **UnitOfWork per request**: A single `SaveChangesAsync` call per operation, after all entity mutations are complete.
5. **Event-driven communication**: No synchronous inter-service commands. Events carry all data the consumer needs.
