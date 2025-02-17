using DatabaseService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using DatabaseService;

var mongoService = new MongoService("mongodb://localhost:27017/", "foodFacts");
var foodItemRepository = new DatabaseService<FoodItem>(mongoService);