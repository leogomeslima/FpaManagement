using System.Text;
using System.Text.Json;
using FpaManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FpaManagement.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public CacheService(IDistributedCache cache, IConfiguration configuration, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;

        var defaultExpirationMinutes = configuration.GetValue<int>("Cache:DefaultExpirationMinutes", 60);
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(defaultExpirationMinutes)
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetAsync(key, cancellationToken);
            if (value == null)
            {
                return default;
            }

            var json = Encoding.UTF8.GetString(value);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = expiration.HasValue
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
                : _defaultOptions;

            var json = JsonSerializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);

            await _cache.SetAsync(key, bytes, options, cancellationToken);

            _logger.LogDebug("Cache set for key {Key} with expiration {Expiration}", key, expiration ?? _defaultOptions.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Cache removed for key {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetAsync(key, cancellationToken);
            return value != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key {Key}", key);
            return false;
        }
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }
}
