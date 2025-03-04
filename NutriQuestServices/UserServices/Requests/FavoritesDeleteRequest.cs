namespace NutriQuestServices.UserServices.Requests;

public class FavoritesDeleteRequest
{
    public required string UserId { get; set; }

    public required string ItemId { get; set; }
}
