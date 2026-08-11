namespace ServerApp.Infrastructure.Services;

using Microsoft.Extensions.Caching.Memory;
using ServerApp.Application.Services;

public class StateStore : IStateStore
{
    private readonly IMemoryCache _cache;

    private const string StateCachePrefix = "oauth_state_";
    private static readonly TimeSpan StateExpiration = TimeSpan.FromMinutes(10);

    public StateStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void StoreState(string state)
    {
        _cache.Set($"{StateCachePrefix}{state}", state, StateExpiration);
    }

    public bool ValidateAndRemoveState(string state)
    {
        if (_cache.TryGetValue($"{StateCachePrefix}{state}", out _))
        {
            _cache.Remove($"{StateCachePrefix}{state}");
            return true;
        }
        return false;
    }
}
