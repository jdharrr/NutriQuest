using CacheServices;
using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;
using NutriQuestServices.FoodServices.FoodItemProjections;
using NutriQuestServices.FoodServices.FoodRequests;
using NutriQuestServices.FoodServices.FoodResponses;
using NutriQuestServices.FoodServices.Responses;
using System.Text.RegularExpressions;

namespace NutriQuestServices.FoodServices;

public class FoodService
{
    private readonly DatabaseService<FoodItem> _dbService;

    private readonly CacheService _cache;

    private readonly string _idsShownKey = "idsShown";

    private readonly int _itemsPerPage = 9;

    private readonly string _imageBaseUrl = "https://images.openfoodfacts.org/images/products";

    private readonly string _frontImageName = "front_en";

    private readonly string _nutritionImageName = "nutrition_en";

    private readonly string _ingredientsImageName = "ingredients_en";

    private readonly string _packagingImageName = "packaging_en";

    private readonly string _barcodeSplitPattern = @"^(...)(...)(...)(.*)$";

    public FoodService(DatabaseService<FoodItem> databaseService, CacheService cache)
    {
        _dbService = databaseService;
        _cache = cache;
    }

    // **********************
    // Used by the controller
    // **********************

    public async Task<FoodItemByIdResponse> GetFoodItemByIdAsync(FoodItemByIdRequest request)
    {
        var response = new FoodItemByIdResponse();

        var findOptions = new FindOptions<FoodItem>
        {
            Projection = Builders<FoodItem>.Projection.Exclude(x => x.Images)
                                                      .Exclude(x => x.MaxImgId)
                                                      .Exclude(x => x.Rev)
        };
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Id, request.ItemId);

        response.FoodItem = await _dbService.FindOneAsync(filter, findOptions).ConfigureAwait(false);

        return response;
    }

    public async Task<FoodItemFrontImgResponse> GetFoodItemFrontImgUrlAsync(FoodImageRequest request)
    {
        var response = new FoodItemFrontImgResponse();

        var imageFilter = Builders<FoodItem>.Filter.Eq(x => x.Id, request.ItemId);
        var findOptions = new FindOptions<FoodItem, FoodItemImageProjection>
        {
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemImageProjection
                {
                    Id = x.Id,
                    Images = x.Images ?? new List<Image>(),
                    Code = x.Code ?? string.Empty,
                    Rev = x.Rev ?? 0,
                }
            )
        };
        var item = await _dbService.FindOneAsync(imageFilter, findOptions).ConfigureAwait(false);
        if (item == null)
            return response;

        response.Url = BuildImageUrl(item, ImageType.Front);

        return response;
    }

    public async Task<FoodItemAllImgResponse?> GetFoodItemAllImgUrlsAsync(FoodImageRequest request)
    {
        var response = new FoodItemAllImgResponse();

        var imageFilter = Builders<FoodItem>.Filter.Eq(x => x.Id, request.ItemId);
        var findOptions = new FindOptions<FoodItem, FoodItemImageProjection>
        {
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemImageProjection
                {
                    Id = x.Id,
                    Images = x.Images ?? new List<Image>(),
                    Code = x.Code ?? string.Empty,
                    Rev = x.Rev ?? 0,
                }
            )
        };
        var item = await _dbService.FindOneAsync(imageFilter, findOptions).ConfigureAwait(false);
        if (item == null)
            return null;

        var imageTypes = item.Images.Select(x => x.ImageType);
        if (imageTypes == null)
            return null;

        foreach (var type in imageTypes)
        {
            var url = BuildImageUrl(item, (ImageType)type!);
            if (string.IsNullOrEmpty(url))
                continue;

            var imageDetails = new ImageDetails
            {
                Url = url,
                ImageType = type.ToString()!
            };
            response.Images.Add(imageDetails);
        }

        return response;
    }

    public async Task<List<FoodItemPreviewsResponse>> GetFoodItemPreviewsAsync(FoodItemPreviewsRequest request)
    {
        var sessionId = request.SessionId;
        var prevPage = request.PrevPage;

        var idsShownValue = await _cache.GetCacheValue($"{_idsShownKey}-{sessionId}").ConfigureAwait(false);
        List<string> idsShown = [];
        if (!string.IsNullOrEmpty(idsShownValue))
            idsShown = [.. idsShownValue.Split(',')];

        var findOptions = new FindOptions<FoodItem, FoodItemPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemPreviewsResponse
                {
                    Id = x.Id,
                    Name = x.ProductName,
                    Price = x.Price,
                    StoresInStock = x.StoresInStock,
                    Brands = x.Brands,
                    Rating = x.Rating
                }
            )
        };

        FilterDefinition<FoodItem> filter;
        if (idsShown.Count == 0 || idsShown.Count == 1 && prevPage)
        {
            filter = Builders<FoodItem>.Filter.Empty;
        }
        else if (prevPage == true)
        {
            if (idsShown.Count < 3)
            {
                idsShown.RemoveAt(1);
                filter = Builders<FoodItem>.Filter.Empty;
            }
            else
            {
                var prevLastIdShown = idsShown[^3];
                idsShown.RemoveAt(idsShown.Count - 1);
                filter = Builders<FoodItem>.Filter.Gt(x => x.Id, prevLastIdShown);
            }
        }
        else
        {
            filter = Builders<FoodItem>.Filter.Gt(x => x.Id, idsShown.Last());
        }

        var foodItems = await _dbService.FindAsync(filter, findOptions).ConfigureAwait(false);
        if (foodItems.Count == 0)
            return [];

        if (!prevPage)
            idsShown.Add(foodItems.Last().Id!);

        await _cache.SetCacheValue($"{_idsShownKey}-{sessionId}", string.Join(',', idsShown), 45).ConfigureAwait(false);

        return foodItems;
    }

    private string BuildImageUrl(FoodItemImageProjection item, ImageType type)
    {
        var rev = item.Images.Where(x => x.ImageType == type).FirstOrDefault()?.Rev;
        if (rev == null)
            return string.Empty;

        var barcode = item.Code.PadLeft(13, '0');
        var splitMatch = Regex.Match(barcode, _barcodeSplitPattern);

        string imageName = string.Empty;
        switch (type)
        {
            case ImageType.Front:
                imageName = _frontImageName;
                break;
            case ImageType.Nutrition:
                imageName = _nutritionImageName;
                break;
            case ImageType.Ingredients:
                imageName = _ingredientsImageName;
                break;
            case ImageType.Packaging:
                imageName = _packagingImageName;
                break;
        }

        var folderName = $"{splitMatch.Groups[1].Value}/{splitMatch.Groups[2].Value}/{splitMatch.Groups[3].Value}/{splitMatch.Groups[4].Value}";
        var fileName = $"{imageName}.{rev}.400.jpg";

        return $"{_imageBaseUrl}/{folderName}/{fileName}";
    }

    // TODO: Add AddRatingToItem()
    // TODO: Add GetRatingInfo()

    // **********************
    // User by other services
    // **********************

    public async Task<List<FoodItemPreviewsResponse>> GetFoodItemPreviewsByIdsAsync(List<string> ids)
    {
        if (ids.Count < 1)
            return [];

        var findFilter = Builders<FoodItem>.Filter.In(x => x.Id, ids);
        var findOptions = new FindOptions<FoodItem, FoodItemPreviewsResponse>
        {
            Limit = _itemsPerPage,
            Projection = Builders<FoodItem>.Projection.Expression(x =>
                new FoodItemPreviewsResponse
                {
                    Id = x.Id,
                    Name = x.ProductName,
                    Price = x.Price,
                    StoresInStock = x.StoresInStock,
                    Brands = x.Brands,
                    Rating = x.Rating
                }
           )
        };

        return await _dbService.FindAsync(findFilter, findOptions).ConfigureAwait(false);
    }
}