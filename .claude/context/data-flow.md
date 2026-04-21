# Data Flow Documentation

## Order Flow (End-to-End)

The complete journey from adding items to cart through payment completion:

```
1. User adds items to cart
   Frontend -> Gateway -> Basket/CartController -> CartManager
   CartManager validates product exists (REST call to Catalog)
   Cart stored in Basket DB

2. User places order
   Frontend -> Gateway -> Basket/BasketController -> OrderManager
   OrderManager:
     - Creates Order entity from Cart
     - Publishes OrderCreatedEvent to RabbitMQ
     - Returns order confirmation

3. Payment processes order
   RabbitMQ -> Payment/OrderCreatedEventConsumer
   PaymentHandler:
     - Creates PaymentTransaction (status: Pending)
     - Processes payment (external gateway integration)
     - On success: publishes PaymentCompletedEvent
     - On failure: publishes PaymentFailedEvent

4a. Payment Success
   RabbitMQ -> Catalog/PaymentCompletedEventConsumer
     - Decrements StockQuantity for each OrderItem
     - Invalidates product cache
     - Publishes StockUpdatedEvent
   
   RabbitMQ -> Basket/PaymentCompletedEventConsumer
     - Marks order as Paid
     - Clears user's cart

4b. Payment Failure
   RabbitMQ -> Basket/PaymentFailedEventConsumer
     - Marks order as Failed
     - Restores cart state
```

---

## RabbitMQ Event Flow

### Exchange Configuration
- Name: `eticaret_event_bus`
- Type: Topic
- Durable: true

### Event Routing

```
OrderCreatedEvent
  Publisher: Basket Service
  Queue: OrderCreatedEvent_queue
  Consumer: Payment Service
  Payload: { OrderId, UserId, TotalPrice, Items[], CreatedDate }

PaymentCompletedEvent
  Publisher: Payment Service
  Queue: PaymentCompletedEvent_queue
  Consumers: Catalog Service (stock), Basket Service (cart cleanup)
  Payload: { OrderId, UserId, Amount, TransactionId, CompletedDate }

PaymentFailedEvent
  Publisher: Payment Service
  Queue: PaymentFailedEvent_queue
  Consumer: Basket Service
  Payload: { OrderId, UserId, Reason, FailedDate }

StockUpdatedEvent
  Publisher: Catalog Service
  Queue: StockUpdatedEvent_queue
  Consumers: (future: Notification Service)
  Payload: { ProductId, NewStockQuantity, UpdatedDate }
```

### Message Lifecycle

```
Publisher                    RabbitMQ                     Consumer
    |                          |                             |
    |-- PublishAsync(event) -->|                             |
    |                          |-- Route to queue ---------->|
    |                          |                             |-- Deserialize
    |                          |                             |-- HandleAsync()
    |                          |                             |-- BasicAck (success)
    |                          |                             |-- BasicNack+requeue (failure)
    |                          |<-- ACK/NACK ---------------|
```

---

## Caching Flow (Redis)

### Read Path (Cache-Aside)

```
Client Request
    |
    ▼
Service.GetByIdAsync(id)
    |
    ├── cacheService.GetAsync(key)
    │       |
    │       ├── HIT  -> return cached data
    │       │
    │       └── MISS -> repository.GetWithAsNoTrackingAsync()
    │                       |
    │                       ├── NOT FOUND -> throw NotFoundException
    │                       │
    │                       └── FOUND -> mapper.Map<Dto>(entity)
    │                                       |
    │                                       ├── cacheService.SetAsync(key, dto, ttl)
    │                                       └── return dto
    ▼
ApiResponse<T>.Success(data)
```

### Write Path (Cache Invalidation)

```
Service.UpdateAsync(dto)
    |
    ├── repository.GetAsync() (tracked)
    ├── mapper.Map(dto, entity)
    ├── repository.UpdateAsync(entity)
    ├── unitOfWork.SaveChangesAsync()
    ├── cacheService.RemoveAsync(key)  ← INVALIDATE
    └── return ApiResponse.Success(result)
```

### Cache Key Format

```
{service}:{entity}:{identifier}

Examples:
  catalog:product:42
  catalog:brand:7
  catalog:category:15
  basket:cart:user_123
```

---

## Elasticsearch Search Flow

### Indexing (Write)

```
ProductManager.CreateAsync()
    |
    ├── Save to PostgreSQL (source of truth)
    ├── Map entity -> ProductSearchDocument
    └── searchService.IndexDocumentAsync(document)
            |
            └── ES PUT /catalog_products/_doc/{id}
```

### Search (Read)

```
Client: GET /api/catalog/products/search?query=phone&minPrice=100

ProductsController.Search()
    |
    └── searchService.SearchAsync(searchDto)
            |
            ├── Build Bool query (must + filter)
            │     must: MultiMatch on [name^3, description, code^2]
            │     filter: TermQuery(categoryId), RangeQuery(price)
            │
            ├── ES POST /catalog_products/_search
            │
            └── Map response -> PagedResult<ProductSearchDocument>
```

### Data Consistency

- PostgreSQL is the **source of truth**
- Elasticsearch is eventually consistent (updated on write operations)
- If ES index becomes stale, a full reindex job can rebuild from PostgreSQL
- Read operations that require consistency (e.g., stock check before purchase) must hit PostgreSQL, not ES

---

## API Request Flow

```
Client Request
    |
    ▼
API Gateway (Ocelot :5000)
    |
    ├── Route matching (ocelot.json)
    ├── Forward to downstream service
    |
    ▼
Controller (thin)
    |
    ├── var result = await _service.MethodAsync(dto, cancellationToken)
    └── return StatusCode(result.StatusCode, result)
    |
    ▼
Service Manager
    |
    ├── Predicate.GetExpression(filterDto)  ← filtering
    ├── Order.GetOrder(orderBy, orderType)   ← sorting
    ├── Selector.GetSelector()               ← projection
    ├── repository.GetAll...Async()          ← data access
    ├── mapper.Map<Dto>(entity)              ← mapping
    ├── cacheService.Get/Set/RemoveAsync()   ← caching
    ├── eventBus.PublishAsync()              ← eventing
    └── ApiResponse<T>.Success/Fail()        ← response
    |
    ▼
Repository (EfEntityRepositoryBase)
    |
    ├── DbContext.Set<T>()
    ├── .Where(predicate)
    ├── .OrderBy/OrderByDescending(order)
    ├── .Select(selector)
    ├── .Skip/Take (pagination)
    └── .ToListAsync(cancellationToken)
    |
    ▼
PostgreSQL
```
