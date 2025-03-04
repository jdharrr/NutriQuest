namespace NutriQuestServices.UserServices.Requests;

public class AddToCartRequest
{
    public required string UserId { get; set; }

    public required string ItemId { get; set; }
}
