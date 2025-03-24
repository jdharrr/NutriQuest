namespace DatabaseServices.Models;

public class Ingredient : BaseModel, IMongoDocument
{
    public static string CollectionName => "ingredients";

    public List<string> AllIngredients = [];
}
