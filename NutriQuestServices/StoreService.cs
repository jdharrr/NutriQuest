using NutriQuestRepositories;

namespace NutriQuestServices;

public class StoreService
{
    private readonly StoreRepository _storeRepo;
    
    public StoreService(StoreRepository storeRepo)
    {
        _storeRepo = storeRepo;
    }
}