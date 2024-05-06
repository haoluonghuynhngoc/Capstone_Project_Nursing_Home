using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nursing_Home.Application.Services;
using System.Collections.Concurrent;

namespace Nursing_Home.Infrastructure.Services;
public class CacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CachKeys = new();

    private readonly IDistributedCache _distributedCache;
    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        string? cacheEntity = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (cacheEntity is null)
        {
            return null;
        }
        T? result = JsonConvert.DeserializeObject<T>(cacheEntity);
        return result;
    }

    public async Task SetDataAsync<T>(string key, T data, int TimeLimit, CancellationToken cancellationToken) where T : class
    {
        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(data), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(TimeLimit)
        }, cancellationToken);

        CachKeys.TryAdd(key, false);
    }
    // xóa cache theo key
    public async Task RemoveDataAsync(string key, CancellationToken cancellationToken)
    {
        //var cacheEntity = _distributedCache.GetString(key);
        //if (!string.IsNullOrEmpty(cacheEntity))
        //{
        //    _distributedCache.Remove(key);       
        //}
        await _distributedCache.RemoveAsync(key, cancellationToken);

        CachKeys.TryRemove(key, out bool _);
    }
    // xóa cache theo key theo tiền tố vd như nó sẽ xóa hết những thằng bắt đầu bằng user_    
    public Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        // cach 1
        //foreach (var key in CachKeys.Keys)
        //{
        //    if (key.StartsWith(prefixKey))
        //    {
        //        RemoveDataAsync(key, cancellationToken);
        //    }
        //}
        //return Task.CompletedTask;
        // cach 2
        //CachKeys.Keys.Where(k => k.StartsWith(prefixKey)).ToList().ForEach(k => CachKeys.TryRemove(k, out bool _));
        IEnumerable<Task> tasks = CachKeys
            .Keys.Where(k => k.StartsWith(prefixKey))
            .Select(k => RemoveDataAsync(k, cancellationToken));
        return Task.WhenAll(tasks);
    }
}
