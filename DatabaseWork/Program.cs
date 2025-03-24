using DatabaseServices;
using DatabaseServices.Models;
using MongoDB.Bson;
using MongoDB.Driver;


//************************************************************************************************************
// Just used to quickly interact with the database... not intended for actual nutriquest services interactions
//************************************************************************************************************


var mongoSettings = new MongoSettings
{
    ConnectionString = "mongodb://localhost:27017/",
    Name = "foodFactsCleaned"
};
var mongoService = new DatabaseWork.LocalServices.MongoService(mongoSettings);
var foodRepo = new DatabaseWork.LocalServices.DatabaseService<FoodItem>(mongoService);
//var tempRepo = new DatabaseWork.DatabaseService<TempRepoData>(mongoService);

//var allergensFound = new List<string>();

var filter = Builders<FoodItem>.Filter.And(
    Builders<FoodItem>.Filter.Regex(x => x.Categories, new BsonRegularExpression(".*(water|mineral-water|sparkling-water|soft-drink|soda|juice|coffee|tea|energy-drinks|beer|wine).*", "i"))
    //Builders<FoodItem>.Filter.Regex(x => x.Categories, new BsonRegularExpression(".*plant-based.*", "i"))
);
var items = await foodRepo.FindAsync(filter);
Console.WriteLine(items);





//foreach (var item in items)
//{
//    foreach (var foodGroup in item.Allergens ?? [])
//    {
//        if (!allergensFound.Contains(foodGroup))
//            allergensFound.Add(foodGroup);
//    }
//}

//var tempData = new TempRepoData
//{
//    AllAllergens = allergensFound
//};

//await tempRepo.InsertOneAsync(tempData);

//public class TempRepoData : BaseModel, IMongoDocument
//{
//    public static string? CollectionName => "tempData";

//    public List<string> AllAllergens { get; set; } = [];

//};