using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace NutriQuest.DatabaseService;

public class MongoService
{
    public IMongoDatabase Database { get; }

    private readonly MongoClient _client;

    public MongoService(string connectionUri, string databaseName)
    {
        // Creates convention that the Pascal case properties in the models will be stored as Camel Case inside of mongo
        var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", conventionPack, _ => true);

        _client = new MongoClient(connectionUri);
        Database = _client.GetDatabase(databaseName);
    }
}
