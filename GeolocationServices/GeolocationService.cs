using DatabaseServices.Models;
using GeolocationServices.Requests;
using GeolocationServices.Responses;
using NutriQuestServices;

namespace GeolocationServices;

public class GeolocationService
{
    private readonly GoogleApi _googleApi;
    private readonly StoreService _storeService;
    
    public GeolocationService(GoogleApi googleApi, StoreService storeService)
    {
        _googleApi = googleApi;
        _storeService = storeService;
    }

    public async Task<NearbyStoresByZipCodeResponse> GetValidStoresForLocationAsync(StoresByZipCodeRequest request)
    {
        var response = new NearbyStoresByZipCodeResponse();

        var foundStores = await _googleApi.GetNearbyStoresAsync(request.ZipCode).ConfigureAwait(false);
        if (foundStores.Count == 0)
            return response;

        response.Stores = await _storeService.GetValidStoresAsync(foundStores).ConfigureAwait(false);

        return response;
    }
}