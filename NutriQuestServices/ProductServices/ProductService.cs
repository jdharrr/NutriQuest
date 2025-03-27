using DatabaseServices.Models;
using NutriQuestRepositories;
using NutriQuestRepositories.ProductRepo;
using NutriQuestRepositories.ProductRepo.Enums;
using NutriQuestRepositories.ProductRepo.Responses;
using NutriQuestServices.ProductServices.Requests;
using NutriQuestServices.UserServices;

namespace NutriQuestServices.ProductServices;

public class ProductService
{
    private readonly ProductRepository _productRepo;

    private readonly UserRepository _userRepo;

    private readonly string _categoryEnumsNamespace;

    private readonly string _categoryEnumsAssembly;

    public ProductService(ProductRepository productRepo, UserRepository userRepo)
    {
        _productRepo = productRepo;
        _userRepo = userRepo;

        // Set the namespace of the food enums so we can ensure we can find the sub categories w/ reflection
        _categoryEnumsNamespace = typeof(MainFoodCategories).Namespace ?? "";
        _categoryEnumsAssembly = typeof(MainFoodCategories).Assembly.ToString();
    }

    public async Task<Product> GetProductByIdAsync(ProductByIdRequest request)
    {
        return await _productRepo.GetProductByIdAsync(request.ProductId).ConfigureAwait(false)
            ?? throw new ProductNotFoundException();
    }

    public async Task<ProductFrontImgResponse> GetProductFrontImgUrlAsync(ImageRequest request)
    {
        var response = new ProductFrontImgResponse();

        var item = await _productRepo.GetProductByIdAsync(request.ProductId).ConfigureAwait(false)
            ?? throw new ProductNotFoundException();
        
        response.Url = _productRepo.BuildImageUrl(item.Images!, item.Code!, ImageType.Front);

        return response;
    }

    public async Task<ProductAllImgResponse> GetProductAllImgUrlsAsync(ImageRequest request)
    {
        var response = new ProductAllImgResponse();

        var item = await _productRepo.GetProductByIdAsync(request.ProductId).ConfigureAwait(false)
            ?? throw new ProductNotFoundException();

        var imageTypes = item.Images?.Select(x => x.ImageType);
        if (imageTypes == null)
            return response;

        response.Images = [];
        foreach (var type in imageTypes)
        {
            var url = _productRepo.BuildImageUrl(item.Images!, item.Code!, (ImageType)type!);
            if (string.IsNullOrEmpty(url))
                continue;

            var imageDetails = new ImageDetails
            {
                Url = url,
                ImageType = type.ToString()!
            };
            response.Images.Add(imageDetails);
        }

        if (response.Images.Count == 0)
        {
            response.Images = null;
        }

        return response;
    }

    public async Task<List<ProductPreviewsResponse>> GetProductPreviewsPagingAsync(ProductPreviewsRequest request)
    {
        return await _productRepo.GetProductPreviewsPagingAsync(request.SessionId, request.PrevPage, request.RestartPaging, request.Filters.MainCategory, request.Filters.SubCategory, request.Filters.Restrictions, request.Filters.ExcludedIngredients, request.Filters.ExcludedCustomIngredients, request.Sort).ConfigureAwait(false);
    }

    public async Task<AddRatingResponse> AddProductRatingAsync(AddRatingRequest request)
    {
        var response = new AddRatingResponse();

        var item = await _productRepo.GetProductByIdAsync(request.ProductId).ConfigureAwait(false)
            ?? throw new ProductNotFoundException();

        if (!item.AllRatings.TryGetValue(request.Rating, out var value))
        {
            item.AllRatings[request.Rating] = 0;
        }
        
        item.AllRatings[request.Rating]++;
        item.NumberOfRatings++;

        int total = 0;
        foreach (var kvp in item.AllRatings)
        {
            total += (kvp.Value * kvp.Key);
        }

        item.Rating = Math.Round((double)total / item.NumberOfRatings, 1);

        response.RatingSuccess = (await _productRepo.UpdateCompleteProductAsync(item).ConfigureAwait(false)).ModifiedCount > 0;

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException("Unable to update user's ratings.");

        var itemRating = new ItemRating
        {
            ItemId = request.ProductId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        user.Ratings.Add(itemRating);

        await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);

        return response;
    }

    public MainFoodCategoriesResponse GetMainFoodCategories()
    {
        var response = new MainFoodCategoriesResponse
        {
            MainFoodCategories = [.. Enum.GetNames(typeof(MainFoodCategories))]
        };

        return response;
    }

    public SubCategoriesResponse GetSubCategoriesForCategory(SubCategoriesRequest request)
    { 
        var response = new SubCategoriesResponse();
        
        var enumType = Type.GetType($"{_categoryEnumsNamespace}.{request.MainCategory}, {_categoryEnumsAssembly}");
        if (enumType != null)
            response.SubFoodCategories = [.. Enum.GetNames(enumType)];
        
        return response;
    }

    public IngredientsResponse GetFoodIngredients()
    {
        var response = new IngredientsResponse()
        {
            Ingredients = [.. Enum.GetNames(typeof(Ingredients))]
        };

        return response;
    }

    public FoodRestrictionsResponse GetFoodRestrictions()
    {
        var response = new FoodRestrictionsResponse()
        {
            FoodRestrictions = [.. Enum.GetNames(typeof(FoodRestrictions))]
        };

        return response;
    }

    public SortOptionsResponse GetSortOptions()
    {
        var response = new SortOptionsResponse
        {
            SortOptions = [.. Enum.GetNames(typeof(SortOptions))]
        };

        return response;
    }
}