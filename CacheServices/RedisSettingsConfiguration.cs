using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CacheServices;

public class RedisSettingsConfiguration : IConfigureOptions<RedisSettings>
{
	private readonly IConfiguration _config;

	public RedisSettingsConfiguration(IConfiguration config)
	{
		_config = config;
	}

	public void Configure(RedisSettings settings)
	{
		settings.RedisConnection = _config["RedisConnection"];
	}
}
