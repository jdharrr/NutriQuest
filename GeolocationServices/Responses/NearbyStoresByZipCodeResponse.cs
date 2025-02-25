using DatabaseServices.Models;

namespace GeolocationServices.Responses;

public class NearbyStoresByZipCodeResponse
{
    public List<Store> Stores { get; set; } = [];
}
