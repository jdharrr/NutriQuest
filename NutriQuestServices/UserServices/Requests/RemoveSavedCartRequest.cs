namespace NutriQuestServices.UserServices.Requests;

public class RemoveSavedCartRequest
{
    public required string UserId { get; set; }

    public required string CartId { get; set; }
}
