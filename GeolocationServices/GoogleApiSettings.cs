using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GeolocationServices;

public class GoogleApiSettings
{
    public required string GoogleApiKey { get; set; }
}