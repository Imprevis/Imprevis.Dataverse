namespace Imprevis.Dataverse;

using Imprevis.Dataverse.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Generic;

internal class DataverseServiceCache(HybridCache hybridCache, Guid organizationId) : IDataverseServiceCache
{
    public async Task<T> GetOrCreate<T>(string key, Func<CancellationToken, ValueTask<T>> factory, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var options = new HybridCacheEntryOptions
        {
            Expiration = duration,
        };

        return await hybridCache.GetOrCreateAsync(fullKey, factory, options, null, cancellationToken);
    }
    public async Task Set<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        var options = new HybridCacheEntryOptions
        {
            Expiration = duration,
        };

        await hybridCache.SetAsync(fullKey, value, options, null, cancellationToken);
    }

    public async Task Remove(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetKey(key);
        await hybridCache.RemoveAsync(fullKey, cancellationToken);
    }

    public async Task Remove(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var fullKeys = keys.Select(x => GetKey(x));
        await hybridCache.RemoveAsync(fullKeys, cancellationToken);
    }

    private string GetKey(string key)
    {
        return $"/{organizationId}/{key}";
    }
}
