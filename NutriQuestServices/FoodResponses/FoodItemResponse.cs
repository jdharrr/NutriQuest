namespace NutriQuestServices.FoodResponses;

public class FoodItemPreviewResponse
{
    public string? Id { get; set; }
    
    public string? Name { get; set; }

    public double? Price { get; set; }
    
    public List<string>? StoresInStock { get; set; }
    
    public string? Brands { get; set; }
    
    public string? Rating { get; set; }
}