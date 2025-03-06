namespace NutriQuestServices.FoodServices.Enums;

// Anything added to these enums must be added to the FoodEnumHelper
public enum MainFoodCategories
{
    All,
    MealsAndFrozenFoods,
    Beverages,
    SnacksAndAppetizers,
    Breakfast,
    CondimentsAndSpreads,
    MeatAndSeafood,
    Dairies,
    BakeryAndDesserts,
    Grains
}

public enum BeverageSubCategories
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

public enum SnacksAndAppetizersSubCategories
{
    All,
    Crackers,
    Chips,
    NutsAndLegumes,
    Popcorn,
    Pretzels,
    Dips
}

public enum BreakfastSubCategories
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

public enum BakeryAndDessertsSubCategories
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

public enum GrainSubCategories
{
    All,
    Pasta,
    Noodles,
    Rice,
    Barley
}

public static class FoodEnumHelper
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

    private static readonly Dictionary<BeverageSubCategories, string> _beverageSubCategories = new()
    {
        { BeverageSubCategories.All, "" },
        { BeverageSubCategories.SweetenedBeverages, ".*(sweetened-beverage).*" },
        { BeverageSubCategories.UnsweetenedBeverages, ".*(unsweetened-beverage).*" },
        { BeverageSubCategories.Waters, ".*(water).*" },
        { BeverageSubCategories.Teas, ".*(tea).*" },
        { BeverageSubCategories.Juices, ".*(juice).*" },
        { BeverageSubCategories.Sodas, ".*(sodas|carbonated-soft-drinks).*" },
        { BeverageSubCategories.Coffee, ".*(coffee|cold-brew|latte).*" },
        { BeverageSubCategories.EnergyDrink, ".*(energy-drink).*" }
    };

    private static readonly Dictionary<SnacksAndAppetizersSubCategories, string> _snacksAndAppetizersSubCategories = new()
    {
        { SnacksAndAppetizersSubCategories.All, "" },
        { SnacksAndAppetizersSubCategories.Crackers, ".*(cracker).*" },
        { SnacksAndAppetizersSubCategories.Chips, ".*(chip|crisps).*" },
        { SnacksAndAppetizersSubCategories.NutsAndLegumes, ".*(nuts|legumes|seeds|trailmix).*" },
        { SnacksAndAppetizersSubCategories.Popcorn, ".*(popcorn).*" },
        { SnacksAndAppetizersSubCategories.Pretzels, ".*(pretzel).*" },
        { SnacksAndAppetizersSubCategories.Dips, ".*(dips|hummus|salsa|cheese-dip).*" }
    };

    private static readonly Dictionary<BreakfastSubCategories, string> _breakfastSubCategories = new()
    {
        { BreakfastSubCategories.All, "" },
        { BreakfastSubCategories.Cereals, ".*(breakfast-cereal).*" },
        { BreakfastSubCategories.Bars, ".*(cereal-bars).*" },
        { BreakfastSubCategories.Oats, ".*(oat|porridge).*" },
        { BreakfastSubCategories.PancakesAndWaffles, ".*(pancake|waffle).*" },
        { BreakfastSubCategories.Yogurt, ".*(yogurt).*" },
        { BreakfastSubCategories.Breads, ".*(toast|breakfast-bread|bagel).*" },
        { BreakfastSubCategories.Spreads, ".*(jam|jelly|nut-butter|honey).*" }
    };

    private static readonly Dictionary<BakeryAndDessertsSubCategories, string> _dessertsAndBakerySubCategories = new()
    {
        { BakeryAndDessertsSubCategories.All, "" },
        { BakeryAndDessertsSubCategories.IceCream, ".*(ice-cream).*" },
        { BakeryAndDessertsSubCategories.BiscuitsAndCakes, ".*(biscuits-and-cakes|cakes|muffins|cupcakes).*" },
        { BakeryAndDessertsSubCategories.Pastries, ".*(pastries|pastry|danish|donuts|doughnuts).*" },
        { BakeryAndDessertsSubCategories.Cookies, ".*(cookie).*" },
        { BakeryAndDessertsSubCategories.Candy, ".*(candies|candy).*" },
        { BakeryAndDessertsSubCategories.Chocolate, ".*(chocolate).*" },
        { BakeryAndDessertsSubCategories.PiesAndTarts, ".*(pies|tarts).*" },
        { BakeryAndDessertsSubCategories.PuddingsAndCustards, ".*(pudding|custard).*" },
        { BakeryAndDessertsSubCategories.Breads, ".*(bread).*" }
    };

    private static readonly Dictionary<GrainSubCategories, string> _grainSubCategories = new()
    {
        { GrainSubCategories.All, "" },
        { GrainSubCategories.Pasta, ".*(pasta).*" },
        { GrainSubCategories.Noodles, ".*(noodle).*" },
        { GrainSubCategories.Rice, ".*(rice).*" },
        { GrainSubCategories.Barley, ".*(barley).*" },
    };

    public static string GetMainFoodCategoryRegex(MainFoodCategories category)
    {
        return _mainFoodCategories[category];
    }

    public static string GetBeverageSubCategoryRegex(BeverageSubCategories category)
    {
        return _beverageSubCategories[category];
    }

    public static string GetSnackAndAppetizersSubCategoryRegex(SnacksAndAppetizersSubCategories category)
    {
        return _snacksAndAppetizersSubCategories[category];
    }

    public static string GetBreakfastSubCategoryRegex(BreakfastSubCategories category)
    {
        return _breakfastSubCategories[category];
    }

    public static string GetDessertsAndBakerySubCategoryRegex(BakeryAndDessertsSubCategories category)
    {
        return _dessertsAndBakerySubCategories[category];
    }

    public static string GetGrainSubCategoryRegex(GrainSubCategories category)
    {
        return _grainSubCategories[category];
    }
}