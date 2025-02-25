using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CacheServices;

public class RedisSettings
{
    public required string RedisConnection { get; set; }
}