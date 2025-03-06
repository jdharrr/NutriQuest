using DatabaseServices;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseWork.LocalServices;

public class MongoService
{
    public IMongoDatabase Database { get; }

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

        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.Name);
    }
}
