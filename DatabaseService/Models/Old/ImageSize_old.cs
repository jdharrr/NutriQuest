using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models.Old;

[BsonIgnoreExtraElements]
public class ImageSize_old
{
    [BsonElement("w")]
    public int? Width { get; set; }

    [BsonElement("h")]
    public int? Height { get; set; }
}
