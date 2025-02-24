using DatabaseServices.Models;
using GeolocationServices.Requests;
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

    public async Task<List<Store>> GetValidStoresForLocationAsync(StoresByZipCodeRequest request)
    {
        var foundStores = await _googleApi.GetNearbyStoresAsync(request.ZipCode).ConfigureAwait(false);
        if (foundStores.Count == 0)
            return [];

        return await _storeService.GetValidStoresAsync(foundStores).ConfigureAwait(false);
    }
}