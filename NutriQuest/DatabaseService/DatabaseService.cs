using MongoDB.Driver;
using NutriQuest.DatabaseService.Models;
using NutriQuest.DatabaseService.Responses;

namespace NutriQuest.DatabaseService;

public class DatabaseService<TModel>
    where TModel : IMongoDocument
{
    private readonly IMongoCollection<TModel> _collection;

    public DatabaseService(MongoService service)
    {
        // gotta be a better way to do this
        // works for now
        var modelInstance = Activator.CreateInstance<TModel>();
        var collectionName = modelInstance.CollectionName;

        _collection = service.Database.GetCollection<TModel>(collectionName);
    }

    public async Task InsertOneAsync(TModel model, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(model, null, cancellationToken);
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

    public async Task<long> DeleteOne(FilterDefinition<TModel> filter, DeleteOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(filter, options, cancellationToken);
        if (!result.IsAcknowledged)
            return 0;
            
        return result.DeletedCount;
    }

    public async Task<long> DeleteManyAsync(FilterDefinition<TModel> filter, DeleteOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteManyAsync(filter, options, cancellationToken);
        if (!result.IsAcknowledged)
            return 0;

        return result.DeletedCount;
    }

    public async Task<UpdateResponse> UpdateOneAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.UpdateOneAsync(filter, update, options, cancellationToken);
        if (!result.IsAcknowledged)
            return new UpdateResponse { MatchedCount = 0, ModifiedCount = 0 };

        return new UpdateResponse { MatchedCount = result.MatchedCount, ModifiedCount = result.ModifiedCount };
    }

    public async Task<UpdateResponse> UpdateManyAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.UpdateManyAsync(filter, update, options, cancellationToken);
        if (!result.IsAcknowledged)
            return new UpdateResponse { MatchedCount = 0, ModifiedCount = 0 };

        return new UpdateResponse { MatchedCount = result.MatchedCount, ModifiedCount = result.ModifiedCount };
    }
}