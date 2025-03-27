using GeolocationServices;
using GeolocationServices.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("storesByZipCode")]
    public async Task<IActionResult> GetNearbyStoresByZipCodeAsync([FromQuery] StoresByZipCodeRequest request)
    {
        try
        {
            return Ok(await _locationService.GetValidStoresForLocationAsync(request).ConfigureAwait(false));
        }
        catch (Exception) 
        {
            return Problem("An error occurred while processing the request.");
        }
    }
}