namespace NutriQuestServices.UserServices.Requests;

public class FavoritesAddRequest
{
    public required string UserId { get; set; }

    public required string ProductId { get; set; }
}
