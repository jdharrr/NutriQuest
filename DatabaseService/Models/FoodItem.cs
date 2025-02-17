using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models;

[BsonIgnoreExtraElements]
public class FoodItem : BaseModel, IMongoDocument
{
    public static string CollectionName => "foodItems";
}
