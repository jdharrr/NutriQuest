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
    public static Dictionary<SortOptions, string> _sortOptions = new()
    {
        { SortOptions.PriceDescending, "price"},
        { SortOptions.PriceAscending, "price" },
        { SortOptions.BrandsAlphabetically, "brands" },
        { SortOptions.ProductNamesAlphabetically, "productName"}
    };

    public static string GetSortProperty(SortOptions sortOptions)
    {
        return "";
    }
}