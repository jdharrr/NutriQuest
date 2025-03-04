using DatabaseServices.Models;

namespace NutriQuestServices.FoodServices.FoodItemProjections;

public class FoodItemImageProjection
{
    public required string Id { get; set; }

    public required List<Image> Images { get; set; }

    public required string Code { get; set; }

    public required int Rev { get; set; }
}
