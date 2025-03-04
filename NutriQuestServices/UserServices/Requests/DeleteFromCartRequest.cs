namespace NutriQuestServices.UserServices.Requests;

public class DeleteFromCartRequest
{
    public required string UserId { get; set; }

    public required string ItemId { get; set; }
}
