namespace NutriQuestServices.UserServices.Requests;

public class UserRatingsRequest
{
    public required string UserId { get; set; }

    public required bool PrevPage { get; set; }
}