namespace ETicaretAPI.Common.Application.Interfaces;

/// <summary>
/// Redis cache servisi interface'i. Tüm servisler cache için bunu kullanır.
/// </summary>
public interface ICacheService
{
  Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
  Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
  Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
  Task RemoveAsync(string key, CancellationToken cancellationToken = default);
  Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
  Task FlushAllAsync();
}
