using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseServices.Models;

[BsonIgnoreExtraElements]
public class ImageSize
{
    public int? Width { get; set; }

    public int? Height { get; set; }
}
