using DatabaseServices;
using DatabaseServices.Models;
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
double[] priceEndings = [0.00, 0.25, 0.49, 0.50, 0.75, 0.95, 0.99];

var bulkOps = new List<WriteModel<Product>>();
foreach (var product in products)
{
    // Base price between 1 and 49 dollars
    var dollars = rand.Next(1, 50);
    
    // Randomly choose a realistic decimal ending
    var cents = priceEndings[rand.Next(priceEndings.Length)];

    var price = Math.Round(dollars + cents, 2);

    var updateFilter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
    var update = Builders<Product>.Update.Set(x => x.Price, price);
    bulkOps.Add(new UpdateOneModel<Product>(updateFilter, update));
}

if (bulkOps.Count > 0)
    await productRepo.BulkWriteAsync(bulkOps).ConfigureAwait(false);