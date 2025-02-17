using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseServices.Models.Old;

[BsonIgnoreExtraElements]
public class ImageSize_old
{
    [BsonElement("w")]
    public int? Width { get; set; }

    [BsonElement("h")]
    public int? Height { get; set; }
}
