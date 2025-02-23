using GeolocationServices;
using Microsoft.AspNetCore.Mvc;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/geolocation")]
public class GeolocationController : ControllerBase
{
    private readonly GeolocationService _locationService; 
    
    public GeolocationController(GeolocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("storesByZipCode/{zipCode:int}")]
    public async Task<IActionResult> GetNearbyStoresByZipCodeAsync(int zipCode)
    {
        var stores = await _locationService.GetValidStoresForLocationAsync(zipCode).ConfigureAwait(false);
        if (stores.Count == 0)
            return NotFound();

        return Ok(stores);
    }
}