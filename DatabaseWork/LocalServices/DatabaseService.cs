using DatabaseServices.Models;
using DatabaseServices.Responses;
using MongoDB.Driver;

namespace DatabaseWork.LocalServices;

public class DatabaseService<TModel>
where TModel : IMongoDocument
{
    private readonly IMongoCollection<TModel> _collection;

    public DatabaseService(MongoService service)
    {
        _collection = service.Database.GetCollection<TModel>(TModel.CollectionName);
    }

    public async Task InsertOneAsync(TModel model, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(model, null, cancellationToken).ConfigureAwait(false);
    }

    public async Task InsertManyAsync(List<TModel> models, InsertManyOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _collection.InsertManyAsync(models, options, cancellationToken).ConfigureAwait(false); ;
    }

    public async Task<TModel?> FindOneAsync(FilterDefinition<TModel> filter, FindOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        return await Find(filter, options).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false); ;
    }
    public async Task<List<TModel>> FindAsync(FilterDefinition<TModel> filter, FindOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        return await Find(filter, options).ToListAsync(cancellationToken).ConfigureAwait(false); ;
    }
    private IFindFluent<TModel, TModel> Find(FilterDefinition<TModel> filter, FindOptions<TModel>? options = null)
    {
        var findFluent = options?.Projection != null ? _collection.Find(filter).Project(options.Projection)
                                                     : _collection.Find(filter);
        if (options != null)
        {
            findFluent.Limit(options.Limit);
            findFluent.Skip(options.Skip);
            findFluent.Sort(options.Sort);
        }

        return findFluent;
    }

    // For finding and projecting to different data types
    public async Task<TProjection> FindOneAsync<TProjection>(FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options, CancellationToken cancellationToken = default)
    {
        return await Find(filter, options).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<TProjection>> FindAsync<TProjection>(FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options, CancellationToken cancellationToken = default)
    {
        return await Find(filter, options).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    private IFindFluent<TModel, TProjection> Find<TProjection>(FilterDefinition<TModel> filter, FindOptions<TModel, TProjection> options)
    {
        var findFluent = _collection.Find(filter).Project(options.Projection);
        findFluent.Limit(options.Limit);
        findFluent.Skip(options.Skip);
        findFluent.Sort(options.Sort);

        return findFluent;
    }

    public async Task<long> DeleteOne(FilterDefinition<TModel> filter, DeleteOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(filter, options, cancellationToken).ConfigureAwait(false); ;
        if (!result.IsAcknowledged)
            return 0;

        return result.DeletedCount;
    }

    public async Task<long> DeleteManyAsync(FilterDefinition<TModel> filter, DeleteOptions? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteManyAsync(filter, options, cancellationToken).ConfigureAwait(false); ;
        if (!result.IsAcknowledged)
            return 0;

        return result.DeletedCount;
    }

    public async Task<UpdateResponse> UpdateOneAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.UpdateOneAsync(filter, update, options, cancellationToken).ConfigureAwait(false); ;
        if (!result.IsAcknowledged)
            return new UpdateResponse { MatchedCount = 0, ModifiedCount = 0 };

        return new UpdateResponse { MatchedCount = result.MatchedCount, ModifiedCount = result.ModifiedCount };
    }

    public async Task<UpdateResponse> UpdateManyAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions<TModel>? options = null, CancellationToken cancellationToken = default)
    {
        var result = await _collection.UpdateManyAsync(filter, update, options, cancellationToken).ConfigureAwait(false); ;
        if (!result.IsAcknowledged)
            return new UpdateResponse { MatchedCount = 0, ModifiedCount = 0 };

        return new UpdateResponse { MatchedCount = result.MatchedCount, ModifiedCount = result.ModifiedCount };
    }
}
