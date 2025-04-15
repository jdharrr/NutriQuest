using DatabaseServices.Models;

namespace NutriQuestServices.UserServices.Requests;

public class AddNutrientsRequest
{
    public required string UserId { get; set; }

    public Nutrients Nutrients { get; set; } = new();
}