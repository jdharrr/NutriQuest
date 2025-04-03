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

    public List<CartProduct> Cart { get; set; } = [];

    public List<SavedCart> SavedCarts { get; set; } = [];

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpiration { get; set; }

    public int NumberInFavorites { get; set; } = 0;

    public int NumberInCart { get; set; } = 0;

    public List<ProductRating> Ratings { get; set; } = [];

    public List<Nutrients> TrackedNutrients { get; set; } = [];

    public int NumberTrackedNutrients { get => TrackedNutrients.Count; }
}

public class CartProduct
{
    public required string ProductId { get; set; }

    public int NumberOfProduct { get; set; } = 1;
}

public class SavedCart : BaseModel
{
    public DateTime Date = DateTime.UtcNow;

    public List<CartProduct> Products { get; set; } = [];
}

public class ProductRating : IComparable<ProductRating>
{
    public required string ProductId { get; set; }

    public required int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int CompareTo(ProductRating? other)
    {
        if (other == null)
            return 1;

        return this.Date.CompareTo(other.Date);
    }
}

public class Nutrients
{
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public double Calories { get; set; } = 0;

    public double Fats { get; set; } = 0;

    public double Proteins { get; set; } = 0;

    public double Carbs { get; set; } = 0;
}