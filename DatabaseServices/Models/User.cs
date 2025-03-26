using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseServices.Models;

[BsonIgnoreExtraElements]
public class User: BaseModel, IMongoDocument
{
    public static string CollectionName => "users";

    public string? Name { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;

    public string? Password { get; set; } = string.Empty;

    public string? Salt { get; set; } = string.Empty;

    public List<string> Favorites { get; set; } = [];

    public List<string> Cart { get; set; } = [];

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpiration { get; set; }

    public int NumberInFavorites { get; set; } = 0;

    public int NumberInCart { get; set; } = 0;

    public List<ItemRating> Ratings { get; set; } = [];
}

public class ItemRating : IComparable<ItemRating>
{
    public required string ItemId { get; set; }

    public required int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int CompareTo(ItemRating? other)
    {
        if (other == null)
            return 1;

        return this.Date.CompareTo(other.Date);
    }
}