namespace NutriQuestServices.ProductServices.Requests;

public class ProductPreviewsRequest
{
    public required string SessionId { get; set; }

    public required bool PrevPage { get; set; }

    public required bool RestartPaging { get; set; }

    public FilterOptions Filters { get; set; } = new();
}

public class FilterOptions
{
    public string? MainCategory { get; set; }

    public string? SubCategory { get; set; }

    public List<string>? Restrictions { get; set; }

    public List<string>? ExcludedIngredients { get; set; }

    public List<string>? ExcludedCustomIngredients { get; set; }
}