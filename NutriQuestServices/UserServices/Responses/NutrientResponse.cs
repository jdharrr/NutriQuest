using DatabaseServices.Models;

namespace NutriQuestServices.UserServices.Responses;

public class NutrientsResponse
{
    public List<Nutrients> TrackedNutrients { get; set; } = [];
}
