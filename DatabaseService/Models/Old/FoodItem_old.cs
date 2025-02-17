using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models.Old;

[BsonIgnoreExtraElements]
public class FoodItem_old : BaseModel, IMongoDocument
{
    public static string CollectionName => "foodItems";

    public string[]? traces_tags { get; set; } = [];

    public string? allergens_from_user { get; set; } = string.Empty;

    public string[]? categories_tags { get; set; } = [];

    public string? product_name { get; set; } = string.Empty;

    public string[]? _keywords { get; set; } = [];

    [BsonElement("max_imgid")]
    public object? maxImgId { get; set; }

    public string? max_imgid => maxImgId?.ToString();

    public string? traces_from_ingredients { get; set; } = string.Empty;

    public string? ingredients_text_with_allergens { get; set; } = string.Empty;

    public string? ingredients_text_with_allergens_en { get; set; } = string.Empty;

    public string? code { get; set; } = string.Empty;

    public string[]? allergens_tags { get; set; } = [];

    public string[]? ingredients_analysis_tags { get; set; } = [];

    public int? rev { get; set; }

    public string? brands { get; set; } = string.Empty;

    public string? brand_owner { get; set; } = string.Empty;

    public string? allergens_from_ingredients { get; set; } = string.Empty;

    public string[]? ingredients_tags { get; set; } = [];

    public string? traces_from_user { get; set; } = string.Empty;

    public string? ingredients_text { get; set; } = string.Empty;

    public string[]? food_groups_tags { get; set; } = [];

    public string? ingredients_text_en { get; set; } = string.Empty;

    public Images_old? images { get; set; }

    public bool? HasMigrated { get; set; }
}
