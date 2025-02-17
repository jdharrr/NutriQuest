using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models;

[BsonIgnoreExtraElements]
public class ImageSize
{
    public int? Width { get; set; }

    public int? Height { get; set; }
}
