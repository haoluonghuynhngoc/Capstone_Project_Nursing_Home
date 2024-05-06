namespace Nursing_Home.Application.Services;
public interface ICacheService
{
    Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken) where T : class;
    Task SetDataAsync<T>(string key, T data, int TimeSpan1, CancellationToken cancellationToken) where T : class;
    Task RemoveDataAsync(string key, CancellationToken cancellationToken);
    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}
