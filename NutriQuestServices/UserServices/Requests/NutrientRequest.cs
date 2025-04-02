namespace NutriQuestServices.UserServices.Requests;

public class NutrientsRequest
{
    public required string UserId { get; set; }

    public required DateRange DateRange { get; set; } = new();
}

public class DateRange
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndDate { get; set; }
}