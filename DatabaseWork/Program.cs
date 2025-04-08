using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Bson;
using MongoDB.Driver;

var mongoSettings = new MongoSettings
{
    ConnectionString = "",
    Name = "nutriQuest"
};
var mongoService = new DatabaseWork.LocalServices.MongoService(mongoSettings);
var productRepo = new DatabaseWork.LocalServices.DatabaseService<Product>(mongoService);
var storeRepo = new DatabaseWork.LocalServices.DatabaseService<Store>(mongoService);

var productFilter = Builders<Product>.Filter.Empty;
var products = await productRepo.FindAsync(productFilter).ConfigureAwait(false);

var storeFilter = Builders<Store>.Filter.Empty;
var stores = await storeRepo.FindAsync(storeFilter).ConfigureAwait(false);

var rand = new Random();

var inclusionProbability = 0.4;

var bulkOps = new List<WriteModel<Product>>();
foreach (var product in products)
{
    List<string> storesInStock = [];
    foreach (var store in stores)
    {
        if (rand.NextDouble() < inclusionProbability)
        {
            storesInStock.Add(store.Id);
        }
    }

    var updateFilter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
    var update = Builders<Product>.Update.Set(x => x.StoresInStock, storesInStock);
    bulkOps.Add(new UpdateOneModel<Product>(updateFilter, update));
}

if (bulkOps.Count > 0)
    await productRepo.BulkWriteAsync(bulkOps).ConfigureAwait(false);