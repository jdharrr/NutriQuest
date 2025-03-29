namespace NutriQuestServices.UserServices.Requests;

public class ClearCartRequest
{
    public required string UserId { get; set; }

    public required string ProductId { get; set; }
}
