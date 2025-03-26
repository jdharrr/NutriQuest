namespace NutriQuestServices.UserServices.Responses;

public class UserRatingsResponse
{
    public List<RatingInfo> Ratings { get; set; } = [];
}

public class RatingInfo
{
    public string? ProductName { get; set; }

    public string? ImageUrl { get; set; }

    public double? Rating { get; set; } 

    public DateTime? Date { get; set; }

    public string? Comment { get; set; }
}