namespace NutriQuestServices.ProductServices;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException() : base("Product not found.") { }

    public ProductNotFoundException(string message) : base(message) { }
}

public class ProductExistsException : Exception
{
    public ProductExistsException() : base("Product already Exists.") { }

    public ProductExistsException(string? message) : base(message) { }
}