namespace NutriQuestRepositories.ProductRepo.Responses;

public class CartPreviewsResponse
{
    public string? Id { get; set; }

    public string? ProductName { get; set; }

    public double? Price { get; set; }

    public List<string>? StoresInStock { get; set; }

    public int NumberOfProduct { get; set; }
}
