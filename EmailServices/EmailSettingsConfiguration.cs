using EmailServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EmailServices;

public class EmailSettingsConfiguration : IConfigureOptions<EmailSettings>
{
    private readonly IConfiguration _config;

	public EmailSettingsConfiguration(IConfiguration config)
    {
        _config = config;
    }

    public void Configure(EmailSettings settings)
    {
        settings.Email = _config["Email"];
        settings.Password = _config["EmailPassword"];
    }
}
