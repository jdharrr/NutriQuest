using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseServices.Models;

[BsonIgnoreExtraElements]
public class Product : BaseModel, IMongoDocument
{
    public static string CollectionName => "products";

    public List<string>? Traces { get; set; } = [];

    public string? AllergensFromUser { get; set; } = string.Empty;

    public List<string>? Categories { get; set; } = [];

    public string? ProductName { get; set; } = string.Empty;

    public List<string>? Keywords { get; set; } = [];

    public string? MaxImgId { get; set; } = string.Empty;

    public string? TracesFromIngredients { get; set; } = string.Empty;

    public string? IngredientsTextWithAllergens { get; set; } = string.Empty;

    public string? Code { get; set; } = string.Empty;

    public List<string>? Allergens { get; set; } = [];

    public List<string>? IngredientsAnalysis { get; set; } = [];

    public int? Rev { get; set; }

    public string? Brands { get; set; } = string.Empty;

    public string? BrandOwner { get; set; } = string.Empty;

    public string? AllergensFromIngredients { get; set; } = string.Empty;

    public List<string>? Ingredients { get; set; } = [];

    public string? TracesFromUser { get; set; } = string.Empty;

    public string? IngredientsText { get; set; } = string.Empty;

    public List<string>? FoodGroups { get; set; } = [];

    public List<Image>? Images { get; set; } = [];

    public double? Price { get; set; } = 0.00;

    public Dictionary<int, int> AllRatings = [];

    public double Rating { get; set; } = 0.0;

    public int NumberOfRatings { get; set; } = 0;

    public List<string>? StoresInStock { get; set; } = [];

    public string? ImageUrl { get; set; }
}