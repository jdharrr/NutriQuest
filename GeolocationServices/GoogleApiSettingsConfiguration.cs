using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeolocationServices
{
    public class GoogleApiSettingsConfiguration : IConfigureOptions<GoogleApiSettings>
    {
        private readonly IConfiguration _config;

        public GoogleApiSettingsConfiguration(IConfiguration config)
        {
            _config = config;
        }

        public void Configure(GoogleApiSettings settings)
        {
            settings.GoogleApiKey = _config["GoogleApiKey"];
        }
    }
}
