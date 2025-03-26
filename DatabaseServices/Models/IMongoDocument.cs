namespace DatabaseServices.Models;

public interface IMongoDocument
{
    static abstract string? CollectionName { get; }

    public string Id { get; set; }
}
