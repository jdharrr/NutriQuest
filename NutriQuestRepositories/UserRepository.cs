using DatabaseServices;
using DatabaseServices.Models;
using DatabaseServices.Responses;
using MongoDB.Driver;

namespace NutriQuestRepositories;

public class UserRepository
{
	private readonly DatabaseService<User> _dbService;

	public UserRepository(DatabaseService<User> dbService)
	{
		_dbService = dbService;
    }

	public async Task<User?> GetUserByIdAsync(string id)
	{
		var filter = Builders<User>.Filter.Eq(x => x.Id, id);

		return await _dbService.FindOneAsync(filter).ConfigureAwait(false);
	}

	public async Task<User?> GetUserByEmailAsync(string email)
	{
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);

        return await _dbService.FindOneAsync(filter).ConfigureAwait(false);
    }

	public async Task<UpdateResponse> UpdateCompleteUserAsync(User user)
	{
		return await _dbService.ReplaceOneAsync(user).ConfigureAwait(false);
	}

	public async Task InsertUserAsync(User user)
	{
		await _dbService.InsertOneAsync(user).ConfigureAwait(false);
	}
}
