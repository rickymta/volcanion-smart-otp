using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SmartOTP.Application.Common.Interfaces;

namespace SmartOTP.Infrastructure.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var data = await cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(data))
            return null;

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }

        var data = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, data, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var data = await cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(data);
    }

    public async Task<long> IncrementAsync(string key, int value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var data = await cache.GetStringAsync(key, cancellationToken);
        var currentValue = string.IsNullOrEmpty(data) ? 0 : long.Parse(data);
        var newValue = currentValue + value;

        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }

        await cache.SetStringAsync(key, newValue.ToString(), options, cancellationToken);
        return newValue;
    }
}
