using MongoDB.Driver;
using NutriQuest.DatabaseService;
using NutriQuest.DatabaseService.Models;

var mongoService = new MongoService("mongodb+srv://jharr:52-Ochre-4765@capstone2025.noiwb.mongodb.net/", "nutriQuest");
var foodItemRepository = new DatabaseService<FoodItem>(mongoService);

var foodItem = new FoodItem { Name = "Apple" };

var apple = await foodItemRepository.FindOneAsync(Builders<FoodItem>.Filter.Eq(x => x.Name, "Apple"));