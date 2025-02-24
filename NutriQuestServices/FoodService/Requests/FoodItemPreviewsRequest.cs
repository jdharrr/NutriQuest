namespace NutriQuestServices.FoodService.FoodRequests;

public class FoodItemPreviewsRequest
{
    public required string UserId { get; set; }

    public required bool PrevPage { get; set; }
}
