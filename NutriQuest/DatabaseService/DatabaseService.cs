using MongoDB.Driver;
using NutriQuest.DatabaseService.Models;

namespace NutriQuest.DatabaseService;

public class DatabaseService<TModel>
    where TModel : IMongoDocument
{
    // for testing purposes
    // TODO: Remove
    private readonly string _connectionUri = "";

    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TModel> _collection;

    public DatabaseService(string connectionUri, string databaseName)
    {
        _client = new MongoClient(_connectionUri);
        _database = _client.GetDatabase(databaseName);

        // gotta be a better way to do this
        // works for now
        var modelInstance = Activator.CreateInstance<TModel>();
        var collectionName = modelInstance.CollectionName;

        _collection = _database.GetCollection<TModel>(collectionName);
    }

    // TODO: Check if options can be passed as null or should be checked before passing
    // TODO: Look into the return values of the operations
    // TODO: Should CancellationToken default here or in the controller
    public async Task InsertOneAsync(TModel model, InsertOneOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(model, options, cancellationToken);
    }

    public async Task InsertManyAsync(List<TModel> models, InsertManyOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.InsertManyAsync(models, options, cancellationToken);
    }

    public async Task<TModel?> FindOneAsync(FilterDefinition<TModel> filter, FindOptions? options = null, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(filter, options, cancellationToken)).FirstOrDefault();
    }

    public async Task<List<TModel>> FindAsync(FilterDefinition<TModel> filter, FindOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filter, options).ToListAsync(cancellationToken);
    }

    public async Task DeleteOne(FilterDefinition<TModel> filter, DeleteOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(filter, options, cancellationToken);
    }

    public async Task DeleteManyAsync(FilterDefinition<TModel> filter, DeleteOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteManyAsync(filter, options, cancellationToken);
    }

    public async Task UpdateOneAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.UpdateOneAsync(filter, update, options, cancellationToken);
    }

    public async Task UpdateManyAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.UpdateManyAsync(filter, update, options, cancellationToken);
    }
}