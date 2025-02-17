using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DatabaseServices.Models.Old;

[BsonIgnoreExtraElements]
public class ImageMetadata_old
{
    [BsonElement("geometry")]
    public string? Geometry { get; set; } = string.Empty;

    [BsonElement("x1")]
    public object? x1 { get; set; }

    public string? X1 => x1?.ToString();

    [BsonElement("y1")]
    public object? y1 { get; set; }

    public string? Y1 => y1?.ToString();

    [BsonElement("x2")]
    public object? x2 { get; set; }

    public string? X2 => x2?.ToString();

    [BsonElement("y2")]
    public object? y2 { get; set; }

    public string? Y2 => y2?.ToString();

    // Often has the same style of dynamic "100", "400", etc.
    [BsonElement("sizes")]
    public Dictionary<string, ImageSize_old>? Sizes { get; set; } = [];

    [BsonElement("normalize")]
    public string? Normalize { get; set; } = string.Empty;

    [BsonElement("rev")]
    public int? Revision { get; set; }

    [BsonElement("imgid")]
    public object? imageId { get; set; }

    public string? ImageId => imageId?.ToString();

    [BsonElement("white_magic")]
    public string? WhiteMagic { get; set; } = string.Empty;

    [BsonElement("angle")]
    public int? Angle { get; set; }

    [BsonElement("coordinates_image_size")]
    public object? coordinatesImageSize { get; set; }

    public string? CoordinatesImageSize => coordinatesImageSize?.ToString();
}
