using System.Net.Sockets;
using System.Text.Json;
using ETicaretAPI.Common.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace ETicaretAPI.Common.Application.Services;

public class RedisCacheManager : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly string? _redisConnectionString;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheManager(IDistributedCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _redisConnectionString = configuration["RedisConfiguration:ConnectionString"]
            ?? configuration.GetConnectionString("Redis");
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

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached is not null)
            return cached;

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }

    public async Task FlushAllAsync()
    {
        if (string.IsNullOrEmpty(_redisConnectionString))
            throw new InvalidOperationException("Redis connection string is not configured.");

        var parts = _redisConnectionString.Split(',')[0].Split(':');
        var host = parts[0];
        var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 6379;

        using var client = new TcpClient();
        await client.ConnectAsync(host, port);

        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);

        string? password = null;
        foreach (var part in _redisConnectionString.Split(','))
        {
            if (part.Trim().StartsWith("password=", StringComparison.OrdinalIgnoreCase))
            {
                password = part.Trim()["password=".Length..];
                break;
            }
        }

        if (!string.IsNullOrEmpty(password))
        {
            await writer.WriteLineAsync($"AUTH {password}");
            await reader.ReadLineAsync();
        }

        await writer.WriteLineAsync("FLUSHDB");
        var response = await reader.ReadLineAsync();

        if (response == null || !response.StartsWith("+OK"))
            throw new InvalidOperationException($"Failed to flush Redis database. Response: {response}");
    }
}
