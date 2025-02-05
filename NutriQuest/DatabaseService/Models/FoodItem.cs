using MongoDB.Bson.Serialization.Attributes;

namespace NutriQuest.DatabaseService.Models;

[BsonIgnoreExtraElements]
public class FoodItem : BaseModel, IMongoDocument
{
    public static string CollectionName => "products";
}
