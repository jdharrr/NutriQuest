namespace NutriQuestRepositories.ProductRepo.Responses;

public class CartResponse
{
    public double TotalPrice { get; set; } = 0;

    public List<CartProductPreview> Products { get; set; } = [];
}

public class CartProductPreview
{
    public string? Id { get; set; }

    public string? ProductName { get; set; }

    public double? Price { get; set; }

    public List<string>? StoresInStock { get; set; }

    public int NumberOfProduct { get; set; }
}
