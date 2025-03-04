namespace NutriQuestServices.FoodServices.Requests;

public class FoodItemPreviewsRequest
{
    public required string SessionId { get; set; }

    public required bool PrevPage { get; set; }
}
