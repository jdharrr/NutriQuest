using CacheServices;
using MongoDB.Driver;
using NutriQuestRepositories;
using NutriQuestRepositories.ProductRepo;
using NutriQuestRepositories.ProductRepo.Responses;
using NutriQuestServices.UserServices.Requests;
using NutriQuestServices.UserServices.Responses;

namespace NutriQuestServices.UserServices;

public class UserService
{
    private readonly UserRepository _userRepo;

    private readonly ProductRepository _productRepo;

    private readonly CacheService _cache;

    private readonly int _ratingsPerPage = 3;

    private readonly string _lastPageShownKey = "lastPageNum";

    public UserService(UserRepository userRepo, ProductRepository productRepo, CacheService cacheService)
    {
        _userRepo = userRepo;
        _productRepo = productRepo;
        _cache = cacheService;
    }  

    public async Task<UserAccountResponse> GetUserAccountAsync(UserAccountRequest request)
    {
        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        return new UserAccountResponse
        {
            Name = user.Name,
            Email = user.Email,
            NumberInCart = user.NumberInCart,
            NumberInFavorites = user.NumberInFavorites
        };
    }

    public async Task<FavoritesAddResponse> AddProductToFavoritesAsync(FavoritesAddRequest request)
    {
        var response = new FavoritesAddResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Favorites.Add(request.ProductId);
        user.NumberInFavorites += 1;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.AddSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<FavoritesDeleteResponse> DeleteProductFromFavoritesAsync(FavoritesDeleteRequest request)
    {
        var response = new FavoritesDeleteResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Favorites.Remove(request.ProductId);
        user.NumberInFavorites -= 1;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.DeleteSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<FavoritesClearResponse> ClearFavoritesAsync(FavoritesClearRequest request)
    {
        var response = new FavoritesClearResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Favorites.Clear();
        user.NumberInFavorites = 0;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.ClearSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<List<ProductPreviewsResponse>> GetFavoritesAsync(GetFavoritesRequest request)
    {
        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        return await _productRepo.GetProductPreviewsByIdsAsync(user.Favorites).ConfigureAwait(false);
    }

    public async Task<AddToCartResponse> AddProductToCartAsync(AddToCartRequest request)
    {
        var response = new AddToCartResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Cart.Add(request.ProductId);
        user.NumberInCart += 1;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.AddSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<DeleteFromCartResponse> DeleteProductFromCartAsync(DeleteFromCartRequest request)
    {
        var response = new DeleteFromCartResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Cart.Remove(request.ProductId);
        user.NumberInCart -= 1;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.DeleteSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<ClearCartResponse> ClearCartAsync(ClearCartRequest request)
    {
        var response = new ClearCartResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Cart.Clear();
        user.NumberInCart = 0;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.ClearSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<List<ProductPreviewsResponse>> GetCartAsync(GetCartRequest request)
    {
        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        return await _productRepo.GetProductPreviewsByIdsAsync(user.Cart).ConfigureAwait(false);
    }

    public async Task<UserRatingsResponse> GetUserRatingsAsync(UserRatingsRequest request)
    {
        var response = new UserRatingsResponse();

        int lastPageShown = 0;
        var lastShownValue = await _cache.GetCacheValue($"{_lastPageShownKey}-{request.UserId}").ConfigureAwait(false);
        if (!string.IsNullOrEmpty(lastShownValue) && int.TryParse(lastShownValue, out var value))
            lastPageShown = value;

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        var currentPage = request.PrevPage ? Math.Max(0, lastPageShown - 2) : lastPageShown;

        var ratingsToShow = user.Ratings.OrderBy(x => x).Skip(currentPage * _ratingsPerPage).Take(_ratingsPerPage);

        foreach (var rating in ratingsToShow)
        {
            var item = await _productRepo.GetProductByIdAsync(rating.ProductId, true).ConfigureAwait(false);
            if (item == null)
                continue;
        
            response.Ratings.Add(new RatingInfo
            {
                ProductName = item.ProductName,
                Rating = rating.Rating,
                Comment = rating.Comment,
                Date = rating.Date,
                ImageUrl = item.ImageUrl
            });
        }

        if (ratingsToShow.Any())
        {
            await _cache.SetCacheValue($"{_lastPageShownKey}-{request.UserId}", (currentPage + 1).ToString(), 30).ConfigureAwait(false);
        }

        return response;
    }
}