namespace NutriQuest.DatabaseService.Models;

public interface IMongoDocument
{
    static abstract string? CollectionName { get; }
}
