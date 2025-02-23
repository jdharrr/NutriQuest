namespace NutriQuestServices.FoodResponses;

public class FoodItemAllImgResponse
{
    public List<ImageDetails> Images { get; set; } = [];
}

public class ImageDetails
{
    public required string Url { get; set; }

    public required string ImageType { get; set; }
}