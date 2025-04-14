namespace NutriQuestRepositories.ProductRepo.Responses;

public class ProductPreviewsResponse
{
    public required string Id { get; set; }

    public required string ProductName { get; set; }

    public required double Price { get; set; }

    public List<string> StoresInStock { get; set; } = [];

    public required string Brands { get; set; }

    public required double Rating { get; set; }
}