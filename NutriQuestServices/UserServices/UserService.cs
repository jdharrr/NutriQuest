using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;
using NutriQuestServices.FoodServices;
using NutriQuestServices.FoodServices.Responses;
using NutriQuestServices.UserServices.Requests;
using NutriQuestServices.UserServices.Responses;

namespace NutriQuestServices.UserServices;

public class UserService
{
    private readonly DatabaseService<User> _dbService;

    private readonly FoodService _foodService;

    public UserService(DatabaseService<User> dbService, FoodService foodService)
    {
        _dbService = dbService;
        _foodService = foodService;
    }

    public async Task<UserAccountResponse?> GetUserAccountAsync(UserAccountRequest request)
    {
        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var findOptions = new FindOptions<User, UserAccountResponse>
        {
            Projection = Builders<User>.Projection.Expression(x =>
            new UserAccountResponse {
                Name = x.Name,
                Email = x.Email,
                NumberInCart = x.NumberInCart,
                NumberInFavorites = x.NumberInFavorites
            })
        };

        return await _dbService.FindOneAsync(findFilter, findOptions).ConfigureAwait(false);
    }

    public async Task<FavoritesAddResponse?> AddItemToFavoritesAsync(FavoritesAddRequest request)
    {
        var response = new FavoritesAddResponse();

        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        user.Favorites.Add(request.ItemId);
        user.NumberInFavorites += 1;

        var update = Builders<User>.Update.Set(x => x.Favorites, user.Favorites);
        var updateResponse = await _dbService.UpdateOneAsync(findFilter, update).ConfigureAwait(false);
        response.AddSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<FavoritesDeleteResponse?> DeleteItemFromFavoritesAsync(FavoritesDeleteRequest request)
    {
        var response = new FavoritesDeleteResponse();

        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        user.Favorites.Remove(request.ItemId);
        user.NumberInFavorites -= 1;

        var update = Builders<User>.Update.Set(x => x.Favorites, user.Favorites);
        var updateResponse = await _dbService.UpdateOneAsync(findFilter, update).ConfigureAwait(false);
        response.DeleteSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<FavoritesClearResponse?> ClearFavoritesAsync(FavoritesClearRequest request)
    {
        var response = new FavoritesClearResponse();

        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        user.Favorites.Clear();
        user.NumberInFavorites = 0;

        var update = Builders<User>.Update.Set(x => x.Favorites, user.Favorites);
        var updateResponse = await _dbService.UpdateOneAsync(findFilter, update).ConfigureAwait(false);
        response.ClearSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<List<FoodItemPreviewsResponse>?> GetFavoritesAsync(GetFavoritesRequest request)
    {
        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        return await _foodService.GetFoodItemPreviewsByIdsAsync(user.Favorites).ConfigureAwait(false);
    }

    public async Task<AddToCartResponse?> AddItemToCartAsync(AddToCartRequest request)
    {
        var response = new AddToCartResponse();

        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        user.Cart.Add(request.ItemId);
        user.NumberInCart += 1;

        var update = Builders<User>.Update.Set(x => x.Cart, user.Cart)
                                          .Set(x => x.NumberInCart, user.NumberInCart);
        var updateResponse = await _dbService.UpdateOneAsync(findFilter, update).ConfigureAwait(false);
        response.AddSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<DeleteFromCartResponse?> DeleteItemFromCartAsync(DeleteFromCartRequest request)
    {
        var response = new DeleteFromCartResponse();

        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        user.Cart.Remove(request.ItemId);
        user.NumberInCart -= 1;

        var update = Builders<User>.Update.Set(x => x.Cart, user.Cart)
                                          .Set(x => x.NumberInCart, user.NumberInCart);
        var updateResponse = await _dbService.UpdateOneAsync(findFilter, update).ConfigureAwait(false);
        response.DeleteSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<ClearCartResponse?> ClearCartAsync(ClearCartRequest request)
    {
        var response = new ClearCartResponse();

        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        user.Cart.Clear();
        user.NumberInCart = 0;

        var update = Builders<User>.Update.Set(x => x.Cart, user.Cart)
                                          .Set(x => x.NumberInCart, user.NumberInCart);
        var updateResponse = await _dbService.UpdateOneAsync(findFilter, update).ConfigureAwait(false);
        response.ClearSuccess = updateResponse.ModifiedCount == 1;

        return response;
    }

    public async Task<List<FoodItemPreviewsResponse>?> GetCartAsync(GetCartRequest request)
    {
        var findFilter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbService.FindOneAsync(findFilter).ConfigureAwait(false);
        if (user == null)
            return null;

        return await _foodService.GetFoodItemPreviewsByIdsAsync(user.Cart).ConfigureAwait(false);
    }
}