namespace NutriQuestServices.FoodService.FoodRequests;

public class FoodItemPreviewsRequest
{
    public required string SessionId { get; set; }

    public required bool PrevPage { get; set; }
}
