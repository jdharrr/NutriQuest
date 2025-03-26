using GeolocationServices.Requests;
using GeolocationServices.Responses;
using NutriQuestRepositories;

namespace GeolocationServices;

public class GeolocationService
{
    private readonly GoogleApi _googleApi;
    private readonly StoreRepository _storeRepo;
    
    public GeolocationService(GoogleApi googleApi, StoreRepository storeRepo)
    {
        _googleApi = googleApi;
        _storeRepo = storeRepo;
    }

    public async Task<NearbyStoresByZipCodeResponse> GetValidStoresForLocationAsync(StoresByZipCodeRequest request)
    {
        var response = new NearbyStoresByZipCodeResponse();

        var foundStores = await _googleApi.GetNearbyStoresAsync(request.ZipCode).ConfigureAwait(false);
        if (foundStores.Count == 0)
            return response;

        response.Stores = await _storeRepo.GetStoresByNameAsync(foundStores).ConfigureAwait(false);

        return response;
    }
}