using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DatabaseServices;

public class MongoSettingsConfiguration : IConfigureOptions<MongoSettings>
{
	private readonly IConfiguration _config;

	public MongoSettingsConfiguration(IConfiguration config)
	{
		_config = config;
	}

	public void Configure(MongoSettings settings)
	{
		settings.ConnectionString = _config["MongoConnectionString"];
		settings.Name = _config["DatabaseSettings:Name"];
	}
}
