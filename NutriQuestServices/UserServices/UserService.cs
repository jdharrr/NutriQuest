using CacheServices;
using DatabaseServices.Models;
using MongoDB.Driver;
using NutriQuestRepositories;
using NutriQuestRepositories.ProductRepo;
using NutriQuestRepositories.ProductRepo.Responses;
using NutriQuestServices.ProductServices;
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
            NumberInFavorites = user.NumberInFavorites,
            NumberTrackedNutrients = user.NumberTrackedNutrients
        };
    }

    public async Task<FavoritesAddResponse> AddProductToFavoritesAsync(FavoritesAddRequest request)
    {
        var response = new FavoritesAddResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        if (user.Favorites.Contains(request.ProductId))
            throw new ProductExistsException("Product already exists in favorites.");

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

        if (!user.Favorites.Contains(request.ProductId))
            return response;

        user.Favorites.Remove(request.ProductId);
        user.NumberInFavorites = Math.Max(user.NumberInFavorites - 1, 0);

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

        var product = await _productRepo.GetProductByIdAsync(request.ProductId).ConfigureAwait(false) 
                      ?? throw new ProductNotFoundException();
        
        var productInCart = user.Cart.Products.Find(x => x.ProductId == product.Id);
        if (productInCart == null)
        {
            user.Cart.Products.Add(new CartProduct { ProductId = request.ProductId });
        }
        else
        {
            productInCart.NumberOfProduct++;
        }

        user.Cart.TotalPrice += product.Price ?? 0;
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

        var product = await _productRepo.GetProductByIdAsync(request.ProductId).ConfigureAwait(false)
                      ?? throw new ProductNotFoundException();
        
        var productInCart = user.Cart.Products.Find(x => x.ProductId == request.ProductId);
        if (productInCart == null)
            return response;
        
        if (productInCart.NumberOfProduct == 1)
        {
            user.Cart.Products.Remove(productInCart);
        }
        else
        {
            productInCart.NumberOfProduct--;
        }

        user.Cart.TotalPrice -= product.Price ?? 0;
        user.NumberInCart = Math.Max(user.NumberInCart - 1, 0);

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.DeleteSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<ClearCartResponse> ClearCartAsync(ClearCartRequest request)
    {
        var response = new ClearCartResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        user.Cart.Products.Clear();
        user.Cart.TotalPrice = 0.0;
        user.NumberInCart = 0;

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.ClearSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<CartResponse> GetCartAsync(GetCartRequest request)
    {
        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        return await _productRepo.GetCartPreviewsAsync(user.Cart).ConfigureAwait(false);
    }

    public async Task<SaveCartResponse> SaveCartAsync(SaveCartRequest request)
    {
        var response = new SaveCartResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        var savedCart = new SavedCart
        {
            Cart = user.Cart
        };

        user.SavedCarts.Add(savedCart);

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.SaveCartSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<RemoveSavedCartResponse> RemoveSavedCartAsync(RemoveSavedCartRequest request)
    {
        var response = new RemoveSavedCartResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        var savedCart = user.SavedCarts.Find(x => x.Id == request.CartId);
        if (savedCart == null)
            return response;

        user.SavedCarts.Remove(savedCart);

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.RemoveCartSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<List<SavedCartResponse>> GetSavedCartsAsync(SavedCartsRequest request)
    {
        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
                   ?? throw new UserNotFoundException();

        List<SavedCartResponse> cartResponses = [];
        foreach (var cart in user.SavedCarts)
        {
            var response = new SavedCartResponse
            {
                CartId = cart.Id,
                Date = cart.Date,
                NumberOfProducts = cart.Cart.NumberOfProducts,
                TotalPrice = cart.Cart.TotalPrice
            };

            cartResponses.Add(response);
        }

        return cartResponses;
    }

    public async Task<UserRatingsResponse> GetUserRatingsAsync(UserRatingsRequest request)
    {
        var response = new UserRatingsResponse();

        var lastPageShown = 0;
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

    public async Task<AddNutrientsResponse> AddNutrientsEntryAsync(AddNutrientsRequest request)
    {
        var response = new AddNutrientsResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        var entry = new Nutrients
        {
            Calories = request.Nutrients.Calories,
            Fats = request.Nutrients.Fats,
            Proteins = request.Nutrients.Proteins,
            Carbs = request.Nutrients.Carbs,
            Date = request.Nutrients.Date
        };

        user.TrackedNutrients.Add(entry);

        var updateResponse = await _userRepo.UpdateCompleteUserAsync(user).ConfigureAwait(false);
        response.AddNutrientsSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<NutrientsResponse> GetTrackedNutrientsAsync(NutrientsRequest request)
    {
        var response = new NutrientsResponse();

        var user = await _userRepo.GetUserByIdAsync(request.UserId).ConfigureAwait(false)
            ?? throw new UserNotFoundException();

        List<Nutrients> nutrientsInRange;
        if (request.DateRange.EndDate == null)
        {
            nutrientsInRange = [.. user.TrackedNutrients.Where(x => x.Date.Date == request.DateRange.StartDate.Date)];
        }
        else
        {
            nutrientsInRange = [.. user.TrackedNutrients.Where(x => x.Date.Date >= request.DateRange.StartDate.Date && x.Date.Date <= request.DateRange.EndDate?.Date)];
        }

        response.TrackedNutrients = nutrientsInRange;

        return response;
    }
}