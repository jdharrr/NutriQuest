using DatabaseServices.Models;

namespace NutriQuestRepositories.ProductRepo.Projections;

public class ProductImageProjection
{
    public required string Id { get; set; }

    public required List<Image> Images { get; set; }

    public required string Code { get; set; }
}
