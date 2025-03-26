namespace NutriQuestRepositories.ProductRepo.Responses;

public class ProductAllImgResponse
{
    public List<ImageDetails>? Images { get; set; }
}

public class ImageDetails
{
    public required string Url { get; set; }

    public required string ImageType { get; set; }
}