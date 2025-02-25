using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseServices.Models;

[BsonIgnoreExtraElements]
public class User: BaseModel, IMongoDocument
{
    public static string CollectionName => "users";

    public string? Email { get; set; } = string.Empty;

    public string? Password { get; set; } = string.Empty;

    public string? Salt { get; set; } = string.Empty;

    public List<string> Favorites { get; set; } = [];

    public List<string> Cart { get; set; } = [];
}