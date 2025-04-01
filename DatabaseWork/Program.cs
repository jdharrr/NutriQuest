// using DatabaseServices;
// using DatabaseServices.Models;
// using MongoDB.Bson;
// using MongoDB.Driver;
//
//
// //************************************************************************************************************
// // Just used to quickly interact with the database... not intended for actual nutriquest services interactions
// //************************************************************************************************************
//
//
// var mongoSettings = new MongoSettings
// {
//     ConnectionString = "mongodb+srv://jharr:52-Ochre-4765@capstone2025.noiwb.mongodb.net/",
//     Name = "nutriQuest"
// };
// var mongoService = new DatabaseWork.LocalServices.MongoService(mongoSettings);
// var userRepo = new DatabaseWork.LocalServices.DatabaseService<User>(mongoService);
//

using DatabaseServices.Models;

var itemRatings = new List<ProductRating>();
//
// var rand = new Random();
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6403272b198c24fa740",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6403272b198c24fa741",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa742",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa743",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa744",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa745",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa746",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa747",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa748",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa749",
//     Rating = (int)rand.NextInt64(5),
// });
//
// itemRatings.Add(new ItemRating
// {
//     ItemId = "67b2b6413272b198c24fa74a",
//     Rating = (int)rand.NextInt64(5),
// });
//
// var filter = Builders<User>.Filter.Eq(x => x.Id, "67bcfa2717146e7f1b715a98");
// var update = Builders<User>.Update.Set(x => x.Ratings, itemRatings);
// await userRepo.UpdateOneAsync(filter, update).ConfigureAwait(false);