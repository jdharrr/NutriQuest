using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Driver;

namespace NutriQuestServices;

public class StoreService
{
    private readonly DatabaseService<Store> _dbService;
    
    public StoreService(DatabaseService<Store> dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<Store>> GetValidStoresAsync(List<string> foundStores)
    {
        var filter = Builders<Store>.Filter.In(x => x.Name, foundStores);

        return await _dbService.FindAsync(filter).ConfigureAwait(false);
    }
}