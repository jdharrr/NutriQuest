using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DatabaseServices;

public class MongoService
{
    public IMongoDatabase Database { get; }

    private readonly MongoClient _client;

    public MongoService(MongoSettings settings)
    {
        // Creates convention that the Pascal case properties in the models will be stored as Camel Case inside of mongo
        var camelCasePack = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCasePack, _ => true);

        // Creates the convention that enums will be stored as strings in mongo
        var stringEnumsPack = new ConventionPack { new EnumRepresentationConvention(BsonType.String) };
        ConventionRegistry.Register("StringEnums", stringEnumsPack, _ => true);

        // Creates the convention that null properties will not be added to mongo
        var ignoreNullPack = new ConventionPack { new IgnoreIfNullConvention(true) };
        ConventionRegistry.Register("IgnoreNulls", ignoreNullPack, _ => true);

        _client = new MongoClient(settings.ConnectionString);
        Database = _client.GetDatabase(settings.Name);
    }
}
