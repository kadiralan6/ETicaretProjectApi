# Development Backlog

## P1 — Critical (Current Sprint)

### Elasticsearch Integration for Catalog
- **Service:** Catalog
- **Description:** Implement full-text search for products using Elasticsearch
- **Tasks:**
  - [ ] Create `ProductSearchDocument` model
  - [ ] Implement `IProductSearchService` with ES queries
  - [ ] Add search endpoint to `ProductsController`
  - [ ] Index products on create/update/delete
  - [ ] Build bulk reindex job for initial data load
- **Skill:** `create-elasticsearch-query.md`

### RabbitMQ Consumer for PaymentCompleted in Catalog
- **Service:** Catalog
- **Description:** Consume `PaymentCompletedEvent` to automatically decrement stock
- **Tasks:**
  - [ ] Create `PaymentCompletedEventHandler` in Application
  - [ ] Create `PaymentCompletedEventConsumer` BackgroundService
  - [ ] Register in DI
  - [ ] Test with manual RabbitMQ message publish
- **Skill:** `create-rabbitmq-consumer.md`

### Order Flow in Basket Service
- **Service:** Basket
- **Description:** Complete the order creation flow from cart to event publishing
- **Tasks:**
  - [ ] Implement `OrderManager` with cart-to-order conversion
  - [ ] Validate product availability via REST call to Catalog
  - [ ] Publish `OrderCreatedEvent` after order creation
  - [ ] Add order endpoints to `BasketController`

---

## P2 — Important (Next Sprint)

### Payment Processing Flow
- **Service:** Payment
- **Description:** Implement payment transaction processing
- **Tasks:**
  - [ ] Create `PaymentManager` service
  - [ ] Consume `OrderCreatedEvent`
  - [ ] Implement payment gateway integration (mock for now)
  - [ ] Publish `PaymentCompletedEvent` / `PaymentFailedEvent`
  - [ ] Create payment status endpoint

### JWT Authentication Middleware
- **Service:** All (via Common)
- **Description:** Add JWT validation middleware to protect endpoints
- **Tasks:**
  - [ ] Create shared JWT validation middleware in Common
  - [ ] Add `[Authorize]` attributes to protected endpoints
  - [ ] Configure token validation parameters per service
  - [ ] Add user context extraction (`ICurrentUserService`)

### Global Exception Handling Middleware
- **Service:** All (via Common)
- **Description:** Unified exception-to-ApiResponse mapping
- **Tasks:**
  - [ ] Create `ExceptionHandlingMiddleware`
  - [ ] Map `NotFoundException` -> 404
  - [ ] Map `ValidationException` -> 400
  - [ ] Map unhandled exceptions -> 500 with safe error message
  - [ ] Add structured logging for all exceptions

### Product Reviews
- **Service:** Catalog (or new Review service)
- **Description:** Allow users to review and rate products
- **Tasks:**
  - [ ] Create `Review` entity (rating, comment, userId, productId)
  - [ ] Full CRUD with filtering by product/user
  - [ ] Average rating calculation
  - [ ] Index reviews in Elasticsearch
- **Skill:** `create-entity.md`

---

## P3 — Nice to Have (Future)

### Notification Service
- **Service:** New service (:5005)
- **Description:** Send notifications on order/payment events
- **Tasks:**
  - [ ] Create Notification microservice
  - [ ] Consume `PaymentCompletedEvent`, `StockUpdatedEvent`
  - [ ] Email and push notification support
  - [ ] Notification preferences per user
- **Skill:** `create-service.md`

### Campaign & Discount Engine
- **Service:** Basket
- **Description:** Full campaign/coupon system with discount rules
- **Tasks:**
  - [ ] Campaign rule engine (percentage, fixed, buy-X-get-Y)
  - [ ] Coupon validation and redemption tracking
  - [ ] Cart price recalculation with applied discounts

### Admin Dashboard API
- **Description:** Aggregate endpoints for admin panel
- **Tasks:**
  - [ ] Dashboard stats endpoint (orders, revenue, stock alerts)
  - [ ] Bulk operations (bulk product update, bulk price change)
  - [ ] Audit log queries

### Performance Optimization
- **Tasks:**
  - [ ] Response compression middleware
  - [ ] Rate limiting per endpoint
  - [ ] Database query optimization (analyze slow queries)
  - [ ] Redis cluster configuration for HA
  - [ ] Health check endpoints per service

### Docker & CI/CD
- **Tasks:**
  - [ ] Dockerfile per service
  - [ ] docker-compose for full stack (services + infra)
  - [ ] GitHub Actions CI pipeline (build + test)
  - [ ] CD pipeline for staging deployment

---

## Completed

| Task | Service | Date | Commit |
|---|---|---|---|
| Product Image Management | Catalog | 2026-04-21 | b509304 |
| Catalog DTOs & API improvements | Catalog | 2026-04-20 | fdd7abf |
| Initial Catalog migration | Catalog | 2026-04-19 | ee35fae |
| ApiResponse refactor | All | 2026-04-18 | dc24ed6 |
