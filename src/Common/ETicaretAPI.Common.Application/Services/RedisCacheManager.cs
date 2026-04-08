using System.Text.Json;
using ETicaretAPI.Common.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace ETicaretAPI.Common.Application.Services;

/// <summary>
/// Redis tabanlı cache servisi implementasyonu.
/// </summary>
public class RedisCacheManager : ICacheService
{
  private readonly IDistributedCache _cache;
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public RedisCacheManager(IDistributedCache cache)
  {
    _cache = cache;
  }

  public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
  {
    var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
    if (string.IsNullOrEmpty(cachedValue))
      return default;

    return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
  }

  public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
  {
    var options = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
    };

    var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
    await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
  }

  public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
  {
    await _cache.RemoveAsync(key, cancellationToken);
  }

  public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
  {
    // Not: StackExchange.Redis üzerinden prefix bazlı silme desteklenmez.
    // Production'da Redis SCAN komutu kullanılır.
    return Task.CompletedTask;
  }

  public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
  {
    var value = await _cache.GetStringAsync(key, cancellationToken);
    return !string.IsNullOrEmpty(value);
  }
}
