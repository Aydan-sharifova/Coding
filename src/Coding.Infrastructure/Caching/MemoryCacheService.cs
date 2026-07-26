using System.Collections.Concurrent;
using Coding.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Coding.Infrastructure.Caching;

public sealed class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    private readonly ConcurrentDictionary<string, byte> keys = new(StringComparer.Ordinal);

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue<T>(key, out var cached) && cached is not null)
            return cached;

        var value = await factory(cancellationToken);
        cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime
        });
        keys.TryAdd(key, 0);
        return value;
    }

    public void Remove(string key)
    {
        cache.Remove(key);
        keys.TryRemove(key, out _);
    }

    public void RemoveByPrefix(string prefix)
    {
        foreach (var key in keys.Keys.Where(key =>
                     key.StartsWith(prefix, StringComparison.Ordinal)))
            Remove(key);
    }
}
