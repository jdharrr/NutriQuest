namespace NutriQuest.DatabaseService.Models;

public class FoodItem : BaseModel, IMongoDocument
{
    public string CollectionName => "foodItem";

    public required string Name { get; set; }
}
