using System.Text.Json.Serialization;

namespace GeolocationServices.Responses;

public class NearbyStoresResponse
{
    [JsonPropertyName("places")]
    public required NearbyStores[] Places { get; set; }
}

public class NearbyStores
{
    [JsonPropertyName("displayName")]
    public required StoreDisplayName DisplayName { get; set; }
}

public class StoreDisplayName
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}