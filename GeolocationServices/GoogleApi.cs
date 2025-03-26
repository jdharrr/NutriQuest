using System.Text;
using System.Text.Json;
using GeolocationServices.Requests;
using GeolocationServices.Responses;
using Microsoft.Extensions.Options;

namespace GeolocationServices;

public class GoogleApi
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public GoogleApi(HttpClient httpClient, IOptions<GoogleApiSettings> settings)
    {
        _apiKey = settings.Value.GoogleApiKey;
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetNearbyStoresAsync(int zipCode)
    {
        var zipCodeCoordinates = await GetZipCoordinatesAsync(zipCode).ConfigureAwait(false);
        if (zipCodeCoordinates == null)
            return [];

        var nearbyStores = await GetNearbyStoresResponseAsync(zipCodeCoordinates.Results.First().Geometry.Location.Latitude,
                                                              zipCodeCoordinates.Results.First().Geometry.Location.Longitude
                                                             ).ConfigureAwait(false);
        
        return nearbyStores?.Places.Select(x => x.DisplayName.Text).ToList() ?? [];
    }

    // Potentially get lat and long from user's current location
    // public async Task<List<string>> GetNearbyStores()
    // {
    //     
    // }

    private async Task<NearbyStoresResponse?> GetNearbyStoresResponseAsync(double lat, double lng)
    {
        var requestUri = "https://places.googleapis.com/v1/places:searchNearby";
        var request = new NearbyStoresRequest
        {
            LocationRestriction = new LocationRestriction
            {
                Circle = new LocationCircle
                {
                    Center = new LocationCenter
                    {
                        Latitude = lat,
                        Longitude = lng
                    }
                }
            }
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonRequest = JsonSerializer.Serialize(request, jsonOptions);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", "places.displayName");

        var response = await _httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var responseString =  await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        return JsonSerializer.Deserialize<NearbyStoresResponse>(responseString);
    }

    private async Task<ZipCodeResponse?> GetZipCoordinatesAsync(int zipCode)
    {
        var requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?address={zipCode}&key={_apiKey}";
        var responseString = await _httpClient.GetStringAsync(requestUri).ConfigureAwait(false);
        
        return JsonSerializer.Deserialize<ZipCodeResponse>(responseString);
    }
}
