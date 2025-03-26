using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;

namespace NutriQuestRepositories;

public class StoreRepository
{
	private readonly DatabaseService<Store> _dbService;

	public StoreRepository(DatabaseService<Store> dbService)
	{
		_dbService = dbService;
	}

    public async Task<List<Store>> GetStoresByNameAsync(List<string> names)
    {
        var filter = Builders<Store>.Filter.In(x => x.Name, names);

        return await _dbService.FindAsync(filter).ConfigureAwait(false);
    }
}
