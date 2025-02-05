using MongoDB.Bson;
using MongoDB.Driver;
using NutriQuest.DatabaseService;
using NutriQuest.DatabaseService.Models;

var mongoService = new MongoService("mongodb://localhost:27017/", "foodFacts");
var foodItemRepository = new DatabaseService<FoodItem>(mongoService);

//var response = await foodItemRepository.DeleteManyAsync(
    
//);

Console.WriteLine();