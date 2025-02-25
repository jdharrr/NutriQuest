using GeolocationServices;
using GeolocationServices.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NutriQuestAPI.Controllers;

[Authorize]
[ApiController]
[Route("nutriQuestApi/geolocation")]
public class GeolocationController : ControllerBase
{
    private readonly GeolocationService _locationService; 
    
    public GeolocationController(GeolocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("storesByZipCode")]
    public async Task<IActionResult> GetNearbyStoresByZipCodeAsync([FromQuery] StoresByZipCodeRequest request)
    {
        var response = await _locationService.GetValidStoresForLocationAsync(request).ConfigureAwait(false);
        if (response.Stores.Count == 0)
            return NotFound();

        return Ok(response);
    }
}