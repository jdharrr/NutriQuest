using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseServices.Models;

[BsonIgnoreExtraElements]
public class Store: BaseModel, IMongoDocument
{
    public static string CollectionName => "stores";

    public string Name { get; set; } = string.Empty;
}