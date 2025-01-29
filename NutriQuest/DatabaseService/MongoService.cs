using MongoDB.Driver;

namespace NutriQuest.DatabaseService;

public class MongoService
{
    public IMongoDatabase Database { get; }

    private readonly MongoClient _client;

    public MongoService(string connectionUri, string databaseName)
    {
        _client = new MongoClient(connectionUri);
        Database = _client.GetDatabase(databaseName);
    }
}
