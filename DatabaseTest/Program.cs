using DatabaseService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using DatabaseService;
using DatabaseService.Models.Old;

var mongoServiceOld = new MongoService("mongodb+srv://jharr:52-Ochre-4765@capstone2025.noiwb.mongodb.net/", "nutriQuest");
var oldRepo = new DatabaseService<FoodItem_old>(mongoServiceOld);

var mongoServiceNew = new MongoService("mongodb://localhost:27017/", "foodFactsCleaned");
var newRepo = new DatabaseService<FoodItem>(mongoServiceNew);

// Obsolete
// Used to migrate old data to standardized form
while (true)
{
    var findOptions = new FindOptions<FoodItem_old>
    {
        Limit = 100
    };
    var itemsToMigrate = await oldRepo.FindAsync(
        Builders<FoodItem_old>.Filter.Or(
            Builders<FoodItem_old>.Filter.Eq(x => x.HasMigrated, false),
            Builders<FoodItem_old>.Filter.Eq(x => x.HasMigrated, null)
        )
    );

    if (itemsToMigrate.Count == 0)
        break;

    foreach (var item in itemsToMigrate)
    {
        var cleanedItem = new FoodItem
        {
            Traces = item.traces_tags.ToList(),
            AllergensFromUser = item.allergens_from_user,
            Categories = item.categories_tags.ToList(),
            ProductName = item.product_name,
            Keywords = item._keywords.ToList(),
            MaxImgId = item.max_imgid,
            TracesFromIngredients = item.traces_from_ingredients,
            IngredientsTextWithAllergens = item.ingredients_text_with_allergens_en != string.Empty 
                                                ? item.ingredients_text_with_allergens_en
                                                : item.ingredients_text_with_allergens,
            Code = item.code,
            Allergens = item.allergens_tags.ToList(),
            IngredientsAnalysis = item.ingredients_analysis_tags.ToList(),
            Rev = item.rev,
            Brands = item.brands,
            BrandOwner = item.brand_owner,
            AllergensFromIngredients = item.allergens_from_ingredients,
            Ingredients = item.ingredients_tags.ToList(),
            TracesFromUser = item.traces_from_user,
            IngredientsText = item.ingredients_text_en != string.Empty
                                    ? item.ingredients_text_en
                                    : item.ingredients_text,
            FoodGroups = item.food_groups_tags.ToList(),
        };

        List<Image> images = [];
        if (item.images?.FrontEn != null)
        {
            var front = item.images.FrontEn;
            var image = new Image
            {
                Geometry = front.Geometry,
                X1 = front.X1,
                Y1 = front.Y1,
                X2 = front.X2,
                Y2 = front.Y2,
                Normalize = front.Normalize,
                Rev = front.Revision,
                ImageId = front.ImageId,
                WhiteMagic = front.WhiteMagic,
                Angle = front.Angle,
                CoordinatesImageSize = front.CoordinatesImageSize,
                ImageType = ImageType.Front
            };

            Dictionary<string, ImageSize> sizes = [];
            foreach (var size in front.Sizes ?? [])
            {
                var imageSize = new ImageSize
                {
                    Height = size.Value.Height,
                    Width = size.Value.Width
                };

                sizes[size.Key] = imageSize;
            }

            image.Sizes = sizes;

            images.Add(image);
        }

        if (item.images?.IngredientsEn != null)
        {
            var ingredients = item.images.IngredientsEn;
            var image = new Image
            {
                Geometry = ingredients.Geometry,
                X1 = ingredients.X1,
                Y1 = ingredients.Y1,
                X2 = ingredients.X2,
                Y2 = ingredients.Y2,
                Normalize = ingredients.Normalize,
                Rev = ingredients.Revision,
                ImageId = ingredients.ImageId,
                WhiteMagic = ingredients.WhiteMagic,
                Angle = ingredients.Angle,
                CoordinatesImageSize = ingredients.CoordinatesImageSize,
                ImageType = ImageType.Ingredients
            };

            Dictionary<string, ImageSize> sizes = [];
            foreach (var size in ingredients.Sizes ?? [])
            {
                var imageSize = new ImageSize
                {
                    Height = size.Value.Height,
                    Width = size.Value.Width
                };

                sizes[size.Key] = imageSize;
            }

            image.Sizes = sizes;

            images.Add(image);
        }

        if (item.images?.NutritionEn != null)
        {
            var nutrition = item.images.NutritionEn;
            var image = new Image
            {
                Geometry = nutrition.Geometry,
                X1 = nutrition.X1,
                Y1 = nutrition.Y1,
                X2 = nutrition.X2,
                Y2 = nutrition.Y2,
                Normalize = nutrition.Normalize,
                Rev = nutrition.Revision,
                ImageId = nutrition.ImageId,
                WhiteMagic = nutrition.WhiteMagic,
                Angle = nutrition.Angle,
                CoordinatesImageSize = nutrition.CoordinatesImageSize,
                ImageType = ImageType.Nutrition
            };

            Dictionary<string, ImageSize> sizes = [];
            foreach (var size in nutrition.Sizes ?? [])
            {
                var imageSize = new ImageSize
                {
                    Height = size.Value.Height,
                    Width = size.Value.Width
                };

                sizes[size.Key] = imageSize;
            }

            image.Sizes = sizes;

            images.Add(image);
        }

        if (item.images?.PackagingEn != null)
        {
            var packaging = item.images.PackagingEn;
            var image = new Image
            {
                Geometry = packaging.Geometry,
                X1 = packaging.X1,
                Y1 = packaging.Y1,
                X2 = packaging.X2,
                Y2 = packaging.Y2,
                Normalize = packaging.Normalize,
                Rev = packaging.Revision,
                ImageId = packaging.ImageId,
                WhiteMagic = packaging.WhiteMagic,
                Angle = packaging.Angle,
                CoordinatesImageSize = packaging.CoordinatesImageSize,
                ImageType = ImageType.Packaging
            };

            Dictionary<string, ImageSize> sizes = [];
            foreach (var size in packaging.Sizes ?? [])
            {
                var imageSize = new ImageSize
                {
                    Height = size.Value.Height,
                    Width = size.Value.Width
                };

                sizes[size.Key] = imageSize;
            }

            image.Sizes = sizes;

            images.Add(image);
        }

        cleanedItem.Images = images;

        await newRepo.InsertOneAsync(cleanedItem);
        await oldRepo.UpdateOneAsync(
            Builders<FoodItem_old>.Filter.Eq(x => x.code, item.code),
            Builders<FoodItem_old>.Update.Set(x => x.HasMigrated, true)
        );
    }
}