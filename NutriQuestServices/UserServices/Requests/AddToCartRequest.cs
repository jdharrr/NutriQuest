namespace NutriQuestServices.UserServices.Requests;

public class AddToCartRequest
{
    public required string UserId { get; set; }

    public required string ProductId { get; set; }

    public int NumberOfItem { get; set; } = 1;
}
