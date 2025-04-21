namespace NutriQuestServices.UserServices.Requests;

public class SavedCartRequest
{
    public required string UserId { get; set; }

    public required string CartId { get; set; }
}
