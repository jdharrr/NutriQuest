namespace NutriQuestRepositories.ProductRepo.Responses;

public class ProductPreviewsResponse
{
    public string? Id { get; set; }

    public string? ProductName { get; set; }

    public double? Price { get; set; }

    public List<string>? StoresInStock { get; set; }

    public string? Brands { get; set; }

    public double? Rating { get; set; }
}