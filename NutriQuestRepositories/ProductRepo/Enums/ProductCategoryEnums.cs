namespace NutriQuestRepositories.ProductRepo.Enums;

// Anything added to these enums must be added to the CategoryEnumHelper
public enum MainFoodCategories
{
    All,
    Grains,
    Beverages,
    Breakfast,
    SnacksAndAppetizers,
    BakeryAndDesserts,
    MealsAndFrozenFoods,
    CondimentsAndSpreads,
    MeatAndSeafood,
    Dairies
}

public enum Beverages
{
    All,
    SweetenedBeverages,
    UnsweetenedBeverages,
    Waters,
    Teas,
    Juices,
    Sodas,
    Coffee,
    EnergyDrink
}

public enum SnacksAndAppetizers
{
    All,
    Crackers,
    Chips,
    NutsAndLegumes,
    Popcorn,
    Pretzels,
    Dips
}

public enum Breakfast
{
    All,
    Cereals,
    Bars,
    Oats,
    PancakesAndWaffles,
    Yogurt,
    Breads,
    Spreads
}

public enum BakeryAndDesserts
{
    All,
    IceCream,
    BiscuitsAndCakes,
    Pastries,
    Cookies,
    Candy,
    Chocolate,
    PiesAndTarts,
    PuddingsAndCustards,
    Breads
}

public enum Grains
{
    All,
    Pasta,
    Noodles,
    Rice,
    Barley
}

public static class CategoryEnumHelper
{
    private static readonly Dictionary<MainFoodCategories, string> _mainFoodCategories = new()
    {
        { MainFoodCategories.All, "" },
        { MainFoodCategories.MealsAndFrozenFoods, ".*(meals|frozen|ready-to-eat).*" },
        { MainFoodCategories.Beverages, ".*(beverage).*" },
        { MainFoodCategories.SnacksAndAppetizers, ".*(snack|appetizer).*" },
        { MainFoodCategories.Breakfast, ".*(breakfast).*" },
        { MainFoodCategories.CondimentsAndSpreads, ".*(condiments|spreads).*" },
        { MainFoodCategories.MeatAndSeafood, ".*(meat|seafood|fish|poultry).*" },
        { MainFoodCategories.Dairies, ".*(dairies|dairy|cheese|yogurt|milk).*" },
        { MainFoodCategories.BakeryAndDesserts, ".*(dessert|baking|bread).*" },
        { MainFoodCategories.Grains, ".*(grain).*" },
    };

    private static readonly Dictionary<Beverages, string> _beverageSubCategories = new()
    {
        { Beverages.All, ".*(water|cold-brew|latte|coffee|soft-drink|soda|juice|tea|energy-drink|beer|wine|sweetened-beverage).*" },
        { Beverages.SweetenedBeverages, ".*(sweetened-beverage).*" },
        { Beverages.UnsweetenedBeverages, ".*(unsweetened-beverage).*" },
        { Beverages.Waters, ".*(water).*" },
        { Beverages.Teas, ".*(tea).*" },
        { Beverages.Juices, ".*(juice).*" },
        { Beverages.Sodas, ".*(soda|carbonated-soft-drinks).*" },
        { Beverages.Coffee, ".*(coffee|cold-brew|latte).*" },
        { Beverages.EnergyDrink, ".*(energy-drink).*" }
    };

    private static readonly Dictionary<SnacksAndAppetizers, string> _snacksAndAppetizersSubCategories = new()
    {
        { SnacksAndAppetizers.All, "" },
        { SnacksAndAppetizers.Crackers, ".*(cracker).*" },
        { SnacksAndAppetizers.Chips, ".*(chip|crisps).*" },
        { SnacksAndAppetizers.NutsAndLegumes, ".*(nuts|legumes|seeds|trailmix).*" },
        { SnacksAndAppetizers.Popcorn, ".*(popcorn).*" },
        { SnacksAndAppetizers.Pretzels, ".*(pretzel).*" },
        { SnacksAndAppetizers.Dips, ".*(dips|hummus|salsa|cheese-dip).*" }
    };

    private static readonly Dictionary<Breakfast, string> _breakfastSubCategories = new()
    {
        { Breakfast.All, "" },
        { Breakfast.Cereals, ".*(breakfast-cereal).*" },
        { Breakfast.Bars, ".*(cereal-bars).*" },
        { Breakfast.Oats, ".*(oat|porridge).*" },
        { Breakfast.PancakesAndWaffles, ".*(pancake|waffle).*" },
        { Breakfast.Yogurt, ".*(yogurt).*" },
        { Breakfast.Breads, ".*(toast|breakfast-bread|bagel).*" },
        { Breakfast.Spreads, ".*(jam|jelly|nut-butter|honey).*" }
    };

    private static readonly Dictionary<BakeryAndDesserts, string> _bakeryAndDessertsSubCategories = new()
    {
        { BakeryAndDesserts.All, "" },
        { BakeryAndDesserts.IceCream, ".*(ice-cream).*" },
        { BakeryAndDesserts.BiscuitsAndCakes, ".*(biscuits-and-cakes|cakes|muffins|cupcakes).*" },
        { BakeryAndDesserts.Pastries, ".*(pastries|pastry|danish|donuts|doughnuts).*" },
        { BakeryAndDesserts.Cookies, ".*(cookie).*" },
        { BakeryAndDesserts.Candy, ".*(candies|candy).*" },
        { BakeryAndDesserts.Chocolate, ".*(chocolate).*" },
        { BakeryAndDesserts.PiesAndTarts, ".*(pies|tarts).*" },
        { BakeryAndDesserts.PuddingsAndCustards, ".*(pudding|custard).*" },
        { BakeryAndDesserts.Breads, ".*(bread).*" }
    };

    private static readonly Dictionary<Grains, string> _grainSubCategories = new()
    {
        { Grains.All, "" },
        { Grains.Pasta, ".*(pasta).*" },
        { Grains.Noodles, ".*(noodle).*" },
        { Grains.Rice, ".*(rice).*" },
        { Grains.Barley, ".*(barley).*" },
    };

    public static string GetMainFoodCategoryRegex(string mainCategory)
    {
        if (!Enum.TryParse(typeof(MainFoodCategories), mainCategory, out var value))
            return "";

        return _mainFoodCategories[(MainFoodCategories)value];
    }

    public static string GetSubCategoryRegex(string mainCategory, string subCategory)
    {
        var enumNamespace = typeof(MainFoodCategories).Namespace;
        var subCategoryType = Type.GetType($"{enumNamespace}.{mainCategory}");
        if (subCategoryType == null)
            return "";

        if (!Enum.TryParse(subCategoryType, subCategory, out var value))
            return "";

        string regex = "";
        switch (value)
        {
            case Beverages:
                regex = _beverageSubCategories[(Beverages)value];
                break;
            case SnacksAndAppetizers:
                regex = _snacksAndAppetizersSubCategories[(SnacksAndAppetizers)value];
                break;
            case Breakfast:
                regex = _breakfastSubCategories[(Breakfast)value];
                break;
            case BakeryAndDesserts:
                regex = _bakeryAndDessertsSubCategories[(BakeryAndDesserts)value];
                break;
            case Grains:
                regex = _grainSubCategories[(Grains)value];
                break;
        }

        return regex;
    }
}