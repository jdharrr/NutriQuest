using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models;

public enum ImageType
{
    Front,
    Ingredients,
    Nutrition,
    Packaging
}

[BsonIgnoreExtraElements]
public class Image
{
    public string? Geometry { get; set; } = string.Empty;

    public string? X1 { get; set; } = string.Empty;

    public string? Y1 { get; set; } = string.Empty;

    public string? X2 { get; set; } = string.Empty;

    public string? Y2 { get; set; } = string.Empty;

    public Dictionary<string, ImageSize>? Sizes { get; set; } = [];

    public string? Normalize { get; set; } = string.Empty;

    public int? Rev { get; set; }

    public string? ImageId { get; set; } = string.Empty;

    public string? WhiteMagic { get; set; } = string.Empty;

    public int? Angle { get; set; }

    public string? CoordinatesImageSize { get; set; } = string.Empty;

    public ImageType? ImageType { get; set; }
}
