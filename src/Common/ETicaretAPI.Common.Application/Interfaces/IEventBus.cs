namespace ETicaretAPI.Common.Application.Interfaces;

/// <summary>
/// RabbitMQ event bus interface'i. Servisler arası asenkron iletişim için.
/// </summary>
public interface IEventBus
{
  Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
  void Subscribe<T, THandler>()
      where T : class
      where THandler : IEventHandler<T>;
}

/// <summary>
/// Event handler interface'i.
/// </summary>
public interface IEventHandler<in T> where T : class
{
  Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}
