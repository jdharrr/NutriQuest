using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models.Old;

[BsonIgnoreExtraElements]
public class Images_old
{
    [BsonElement("front_en")]
    public ImageMetadata_old? FrontEn { get; set; }

    [BsonElement("ingredients_en")]
    public ImageMetadata_old? IngredientsEn { get; set; }

    [BsonElement("nutrition_en")]
    public ImageMetadata_old? NutritionEn { get; set; }

    [BsonElement("packaging_en")]
    public ImageMetadata_old? PackagingEn { get; set; }
}
