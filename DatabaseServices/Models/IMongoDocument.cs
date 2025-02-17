namespace DatabaseServices.Models;

public interface IMongoDocument
{
    static abstract string? CollectionName { get; }
}
