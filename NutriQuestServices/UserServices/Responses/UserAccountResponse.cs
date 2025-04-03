namespace NutriQuestServices.UserServices.Responses;

public class UserAccountResponse
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public int NumberInCart { get; set; }

    public int NumberInFavorites { get; set; }

    public int NumberTrackedNutrients { get; set; }
}
