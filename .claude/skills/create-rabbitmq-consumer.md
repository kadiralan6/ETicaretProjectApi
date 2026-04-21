# Skill: Create RabbitMQ Consumer

Add an event consumer that listens to integration events published by another service.

---

## Input Required

- Consumer service name (e.g., Catalog — the service receiving the event)
- Event name (e.g., `PaymentCompletedEvent`)
- Source service (e.g., Payment — the service publishing the event)
- Handler logic description (e.g., "update stock quantities for each item in the order")

---

## Architecture

```
Publisher Service                    Consumer Service
     |                                    |
     |---> RabbitMQ Exchange -----------> Queue ---> Consumer ---> Handler
           (eticaret_event_bus)           ({Event}_queue)
           Topic type
```

- Events are defined in `ETicaretAPI.Common.SharedLibrary.Events`
- Publishing uses `IEventBus.PublishAsync<T>()`
- Consuming uses a hosted service (`BackgroundService`) with RabbitMQ direct channel

---

## Steps

### Step 1: Define Event (if not exists)

File: `src/Common/ETicaretAPI.Common.SharedLibrary/Events/IntegrationEvents.cs`

Add the event class if it doesn't already exist:

```csharp
/// <summary>
/// {Description of when this event fires and who consumes it}
/// {Source} -> {Destination} service(s)
/// </summary>
public class {EventName}
{
    public int {AggregateId} { get; set; }
    // ... event properties
    public DateTime {Action}Date { get; set; } = DateTime.UtcNow;
}
```

Rules:
- Events are immutable facts — past tense naming (Created, Completed, Failed, Updated)
- Include all data the consumer needs — the consumer should not query back to the publisher
- Always include a UTC timestamp

### Step 2: Create Event Handler

File: `src/Services/{ConsumerService}/ETicaretAPI.Services.{ConsumerService}.Application/EventHandlers/{EventName}Handler.cs`

```csharp
using ETicaretAPI.Common.SharedLibrary.Events;

namespace ETicaretAPI.Services.{ConsumerService}.Application.EventHandlers;

public interface I{EventName}Handler
{
    Task HandleAsync({EventName} @event, CancellationToken cancellationToken = default);
}
```

File: `src/Services/{ConsumerService}/ETicaretAPI.Services.{ConsumerService}.Application/EventHandlers/{EventName}HandlerManager.cs`

```csharp
using AutoMapper;
using ETicaretAPI.Common.Application.Interfaces;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.SharedLibrary.Events;
using ETicaretAPI.Services.{ConsumerService}.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Services.{ConsumerService}.Application.EventHandlers;

public class {EventName}HandlerManager : I{EventName}Handler
{
    private readonly I{Entity}Repository _{entityCamel}Repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<{EventName}HandlerManager> _logger;

    public {EventName}HandlerManager(
        I{Entity}Repository {entityCamel}Repository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<{EventName}HandlerManager> logger)
    {
        _{entityCamel}Repository = {entityCamel}Repository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task HandleAsync({EventName} @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling {EventName} for {AggregateId}: {Id}",
            nameof({EventName}), "{AggregateId}", @event.{AggregateId});

        try
        {
            // 1. Load affected entity/entities
            // 2. Apply business logic
            // 3. Persist changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Invalidate cache
            // 5. Optionally publish follow-up events

            _logger.LogInformation("Successfully handled {EventName} for {AggregateId}: {Id}",
                nameof({EventName}), "{AggregateId}", @event.{AggregateId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle {EventName} for {AggregateId}: {Id}",
                nameof({EventName}), "{AggregateId}", @event.{AggregateId});
            throw; // Let the consumer decide retry policy
        }
    }
}
```

### Step 3: Create Background Consumer Service

File: `src/Services/{ConsumerService}/ETicaretAPI.Services.{ConsumerService}.Infrastructure/Consumers/{EventName}Consumer.cs`

```csharp
using System.Text;
using System.Text.Json;
using ETicaretAPI.Common.SharedLibrary.Events;
using ETicaretAPI.Services.{ConsumerService}.Application.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ETicaretAPI.Services.{ConsumerService}.Infrastructure.Consumers;

public class {EventName}Consumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<{EventName}Consumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string ExchangeName = "eticaret_event_bus";
    private const string QueueName = "{EventName}_queue";
    private const string RoutingKey = nameof({EventName});

    public {EventName}Consumer(
        IServiceScopeFactory scopeFactory,
        ILogger<{EventName}Consumer> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(QueueName, ExchangeName, RoutingKey);

        _logger.LogInformation("{Consumer} initialized, bound to queue {Queue}",
            nameof({EventName}Consumer), QueueName);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                var @event = JsonSerializer.Deserialize<{EventName}>(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (@event is null)
                {
                    _logger.LogWarning("Received null {EventName} message, acknowledging and skipping", nameof({EventName}));
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<I{EventName}Handler>();

                await handler.HandleAsync(@event, stoppingToken);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing {EventName}: {Body}", nameof({EventName}), body);
                _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        };

        _channel.BasicConsume(QueueName, autoAck: false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
```

### Step 4: Register in DI

In the consumer service's `Program.cs` or service registration:

```csharp
// Event handler
services.AddScoped<I{EventName}Handler, {EventName}HandlerManager>();

// Background consumer
services.AddHostedService<{EventName}Consumer>();
```

### Step 5: Publish from Source Service (if not already done)

In the source service's Manager:

```csharp
await _eventBus.PublishAsync(new {EventName}
{
    {AggregateId} = entity.Id,
    // ... map properties
});
```

---

## Example: Catalog consuming PaymentCompletedEvent

**Handler** — decrements stock for each item:

```csharp
public async Task HandleAsync(PaymentCompletedEvent @event, CancellationToken cancellationToken = default)
{
    // Fetch order items from event (event carries the data)
    // For each item, reduce stock
    foreach (var item in @event.Items)
    {
        var product = await _productRepository.GetAsync(
            x => x.Id == item.ProductId, cancellationToken: cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found during stock update", item.ProductId);
            continue;
        }

        product.StockQuantity -= item.Quantity;
        await _productRepository.UpdateFieldAsync(product, [x => x.StockQuantity], cancellationToken);
        await _cacheService.RemoveAsync($"catalog:product:{product.Id}", cancellationToken);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);
}
```

---

## Checklist

- [ ] Event class defined in `Common.SharedLibrary.Events`
- [ ] Handler interface + implementation in consumer's Application layer
- [ ] BackgroundService consumer in consumer's Infrastructure layer
- [ ] DI registration for handler (Scoped) and consumer (HostedService)
- [ ] Publisher calls `IEventBus.PublishAsync<T>()` in source service
- [ ] Error handling with logging and NACK+requeue
- [ ] Manual ACK only after successful processing
