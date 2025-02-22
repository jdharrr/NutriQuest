using StackExchange.Redis;

namespace CacheServices;

public class CacheService
{
    private readonly IDatabase _redis;
    
    public CacheService(IConnectionMultiplexer redisService)
    {
        _redis = redisService.GetDatabase(1);
    }

    public async Task SetCacheValue(string key, string value)
    {
        await _redis.StringSetAsync(key, value, TimeSpan.FromHours(1)).ConfigureAwait(false);
    }

    public async Task<string> GetCacheValue(string key)
    {
        return (await _redis.StringGetAsync(key).ConfigureAwait(false)).ToString();
    }
}