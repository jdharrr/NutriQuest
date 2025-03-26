namespace NutriQuestServices.ProductServices;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException() : base("Product not found.") { }

    public ProductNotFoundException(string message) : base(message) { }
}
