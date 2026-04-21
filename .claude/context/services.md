# Service Responsibilities

## Identity Service (:5001)

**Domain:** User management, authentication, authorization

**Entities:**
- `AppUser` — extends IdentityUser, user profile data
- `AppRole` — extends IdentityRole, role definitions
- `Address` — physical/shipping addresses
- `UserAddress` — join table for user-address relationships

**Capabilities:**
- User registration and login (JWT + refresh tokens)
- Password management (change password)
- Role assignment and management
- Address CRUD (create, update, delete, list by user)
- User profile management

**Controllers:**
- `AuthController` — login, register, refresh token
- `UsersController` — user CRUD, role assignment, address management

**Database:** `eticaret_identity` (PostgreSQL)

**Events Published:** None currently
**Events Consumed:** None currently

---

## Catalog Service (:5002)

**Domain:** Product catalog, brands, categories, images

**Entities:**
- `Product` — core product entity (name, price, stock, slug, active/featured flags)
- `Brand` — product brands
- `Category` — product categories (hierarchical via potential parent reference)
- `ProductImage` — product images (URLs from Cloudinary)

**Capabilities:**
- Product CRUD with filtering, pagination, and sorting
- Brand CRUD with filtering
- Category CRUD with filtering
- Product image management (upload via Cloudinary, associate with products)
- Stock quantity updates (partial update via `UpdateFieldAsync`)
- Redis caching for product/brand/category lookups
- Elasticsearch integration (planned/in-progress)

**Controllers:**
- `ProductsController` — full CRUD + stock update + filtered list
- `BrandsController` — full CRUD + filtered list
- `CategoriesController` — full CRUD + filtered list
- `ProductImagesController` — image upload and management

**Database:** `eticaret_catalog` (PostgreSQL)

**External Services:**
- Cloudinary — image upload and storage
- Elasticsearch — product search (in development)

**Events Published:** `StockUpdatedEvent`
**Events Consumed:** `PaymentCompletedEvent` (stock decrement)

---

## Basket Service (:5003)

**Domain:** Shopping cart, orders, coupons, campaigns

**Entities:**
- `Cart` — shopping cart per user
- `OrderItem` — items within a cart/order
- `Order` — placed orders
- `Coupon` — discount coupons
- `Campaign` — promotional campaigns

**Capabilities:**
- Cart management (add/remove/update items)
- Order creation from cart
- Coupon validation and application
- Campaign discount calculation

**Controllers:**
- `BasketController` — cart operations, order placement

**Database:** `eticaret_basket` (PostgreSQL)

**Events Published:** `OrderCreatedEvent` (triggers payment flow)
**Events Consumed:** `PaymentCompletedEvent` (clear cart), `PaymentFailedEvent` (restore cart)

---

## Payment Service (:5004)

**Domain:** Payment processing, transaction tracking

**Entities:**
- `PaymentTransaction` — payment records with status tracking

**Enums:**
- `PaymentStatusEnum` — Pending, Completed, Failed, Refunded
- `PaymentMethodEnum` — CreditCard, DebitCard, BankTransfer, etc.

**Capabilities:**
- Process payments for orders
- Transaction status tracking
- Payment status updates

**Database:** `eticaret_payment` (PostgreSQL)

**Events Published:** `PaymentCompletedEvent`, `PaymentFailedEvent`
**Events Consumed:** `OrderCreatedEvent` (initiate payment processing)

---

## Service Dependency Map

```
Identity ──── (standalone, no service deps)
    │
    │ JWT tokens validated by other services
    ▼
Catalog ◄──── PaymentCompletedEvent (stock decrement)
    │
    │ Product data queried by Basket via REST
    ▼
Basket ────► OrderCreatedEvent ────► Payment
    ◄──── PaymentCompletedEvent ────┘
    ◄──── PaymentFailedEvent ───────┘
```

---

## Service Communication Matrix

| From | To | Method | Event/Endpoint |
|---|---|---|---|
| Basket | Payment | RabbitMQ | `OrderCreatedEvent` |
| Payment | Basket | RabbitMQ | `PaymentCompletedEvent` |
| Payment | Basket | RabbitMQ | `PaymentFailedEvent` |
| Payment | Catalog | RabbitMQ | `PaymentCompletedEvent` (stock update) |
| Catalog | (broadcast) | RabbitMQ | `StockUpdatedEvent` |
| Basket | Catalog | HTTP (REST) | Product details lookup via `IRestService` |
