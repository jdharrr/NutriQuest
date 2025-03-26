namespace NutriQuestServices.ProductServices.Requests;

public class AddRatingRequest
{
    public required string UserId { get; set; }

    public required string ProductId { get; set; }

    public required int Rating { get; set; }

    public string? Comment { get; set; }
}
