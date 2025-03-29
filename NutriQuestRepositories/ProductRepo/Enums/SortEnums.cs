namespace NutriQuestRepositories.ProductRepo.Enums;

public enum SortOptions
{
    PriceDescending,
    PriceAscending,
    BrandsAlphabetically,
    ProductNamesAlphabetically
}

public static class SortOptionsHelper
{
    // These MUST match the properties in the Product model
    private static readonly Dictionary<SortOptions, string> _sortOptions = new()
    {
        { SortOptions.PriceDescending, "price" },
        { SortOptions.PriceAscending, "price" },
        { SortOptions.BrandsAlphabetically, "brands" },
        { SortOptions.ProductNamesAlphabetically, "productName" }
    };

    public static string GetProductPropertyForSort(string sortOption)
    {
        if (!Enum.TryParse(typeof(SortOptions), sortOption, out var value))
            return "";

        return _sortOptions[(SortOptions)value];
    }
}