using Microsoft.AspNetCore.Mvc;
using NutriQuestServices.ProductServices;
using NutriQuestServices.UserServices;
using NutriQuestServices.ProductServices.Requests;

namespace NutriQuestAPI.Controllers;

[ApiController]
[Route("nutriQuestApi/products")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    private readonly string _genericProblemResponse = "An error occurred while processing the request.";

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("productById")]
    public async Task<IActionResult> GetProductByIdAsync([FromQuery] ProductByIdRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ProductId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _productService.GetProductByIdAsync(request).ConfigureAwait(false));
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpPost("productPreviews")]
    public async Task<IActionResult> GetProductPreviewsAsync([FromBody] ProductPreviewsRequest request)
    {
        try
        {
            return Ok(await _productService.GetProductPreviewsPagingAsync(request).ConfigureAwait(false));
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("productFrontImage")]
    public async Task<IActionResult> GetProductFrontImageAsync([FromQuery] ImageRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ProductId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _productService.GetProductFrontImgUrlAsync(request).ConfigureAwait(false));
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("productAllImages")]
    public async Task<IActionResult> GetProductAllImagesAsync([FromQuery] ImageRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ProductId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _productService.GetProductAllImgUrlsAsync(request).ConfigureAwait(false));
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpPost("addProductRating")]
    public async Task<IActionResult> AddProductRatingAsync([FromBody] AddRatingRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(request.ProductId, out var _))
            return BadRequest("Invalid Parameter");

        if (!MongoDB.Bson.ObjectId.TryParse(request.UserId, out var _))
            return BadRequest("Invalid Parameter");

        try
        {
            return Ok(await _productService.AddProductRatingAsync(request).ConfigureAwait(false));
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("mainFoodCategories")]
    public IActionResult GetMainFoodCategories()
    {
        try
        {
            return Ok(ProductService.GetMainFoodCategories());
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("subFoodCategories")]
    public IActionResult GetMainFoodCategories([FromQuery] SubCategoriesRequest request)
    {
        try
        {
            return Ok(_productService.GetSubCategoriesForCategory(request));
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("ingredients")]
    public IActionResult GetIngredients()
    {
        try
        {
            return Ok(ProductService.GetFoodIngredients());
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("foodRestrictions")]
    public IActionResult GetFoodRestrictions()
    {
        try
        {
            return Ok(ProductService.GetFoodRestrictions());
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }

    [HttpGet("sortOptions")]
    public IActionResult GetSortOptions()
    {
        try
        {
            return Ok(ProductService.GetSortOptions());
        }
        catch (Exception)
        {
            return Problem(_genericProblemResponse);
        }
    }
}