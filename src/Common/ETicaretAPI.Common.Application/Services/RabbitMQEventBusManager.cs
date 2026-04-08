using System.Text;
using System.Text.Json;
using ETicaretAPI.Common.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ETicaretAPI.Common.Application.Services;

/// <summary>
/// RabbitMQ tabanlı event bus implementasyonu.
/// Servisler arası asenkron iletişim için kullanılır.
/// </summary>
public class RabbitMQEventBusManager : IEventBus, IDisposable
{
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private readonly IServiceProvider _serviceProvider;
  private readonly string _exchangeName = "eticaret_event_bus";

  public RabbitMQEventBusManager(IConnection connection, IServiceProvider serviceProvider)
  {
    _connection = connection;
    _serviceProvider = serviceProvider;
    _channel = _connection.CreateModel();
    _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
  }

  public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
  {
    var eventName = typeof(T).Name;
    var message = JsonSerializer.Serialize(@event);
    var body = Encoding.UTF8.GetBytes(message);

    var properties = _channel.CreateBasicProperties();
    properties.DeliveryMode = 2; // persistent
    properties.ContentType = "application/json";

    _channel.BasicPublish(
        exchange: _exchangeName,
        routingKey: eventName,
        basicProperties: properties,
        body: body);

    return Task.CompletedTask;
  }

  public void Subscribe<T, THandler>()
      where T : class
      where THandler : IEventHandler<T>
  {
    var eventName = typeof(T).Name;
    var queueName = $"{eventName}_queue";

    _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    _channel.QueueBind(queueName, _exchangeName, eventName);

    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += async (sender, args) =>
    {
      var message = Encoding.UTF8.GetString(args.Body.ToArray());
      var @event = JsonSerializer.Deserialize<T>(message);

      if (@event != null)
      {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<THandler>();
        await handler.HandleAsync(@event);
      }

      _channel.BasicAck(args.DeliveryTag, multiple: false);
    };

    _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
  }

  public void Dispose()
  {
    _channel?.Close();
    _connection?.Close();
  }
}
