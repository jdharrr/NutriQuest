using DatabaseServices;
using DatabaseServices.Models;

namespace NutriQuestServices;

public class UserService
{
    private readonly DatabaseService<User> _dbService;
    
    public UserService(DatabaseService<User> dbService)
    {
        _dbService = dbService;
    }
}