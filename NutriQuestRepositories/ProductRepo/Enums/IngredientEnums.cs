namespace NutriQuestRepositories.ProductRepo.Enums;

// Anything added to these enums must be added to the IngredientEnumHelper
public enum Ingredients
{
    Dairy,
    Egg,
    Shellfish,
    Fish,
    Meat,
    RedMeat,
    Poultry,
    Nuts,
    Legumes,
    Peanuts,
    Soy,
    Sesame,
    Corn,
    Gluten,
    Yeast,
    ArtificialSweeteners,
    AddedSugar,
    Honey,
    Nightshades,
    Gelatin,
    Caffeine,
    Preservatives,
    Colorants,
    MSG,
    Alcohol
}

public enum FoodRestrictions
{
    GlutenFree,
    Vegan,
    Vegetarian,
    Halal,
    Kosher,
    Pescatarian,
    LactoseIntolerant
}

public static class IngredientEnumHelper
{
    private static readonly Dictionary<Ingredients, string> _ingredients = new()
    {
        { Ingredients.Dairy, "\\b(milk|milk\\s+powder|whole\\s+milk|skimmed\\s+milk|dairy|cheese|butter|cream|ghee|yogurt|yoghurt|curd|kefir|paneer|ricotta|mascarpone|casein|caseinate|sodium\\s+caseinate|calcium\\s+caseinate|whey|lactose|buttermilk|milkfat|milk\\s+fat|anhydrous\\s+milk\\s+fat|milk\\s+solids)\\b" },
        { Ingredients.Shellfish, "\\b(shrimp|prawn|crab|lobster|crayfish|krill|langoustine|scampi|clam|mussel|oyster|scallop|squid|calamari|octopus|cuttlefish|snail|escargot|cockle|abalone|whelk|periwinkle)\\b," },
        { Ingredients.Nuts, "\\b(almond|walnut|pecan|hazelnut|filbert|cashew|pistachio|macadamia|brazil\\s+nut|pine\\s+nut|pignolia|chestnut|kola\\s+nut|coconut)\\b"},
        { Ingredients.Legumes, "\\b(peanut|soy|soya|soybean|chickpea|garbanzo|lentil|pea|split\\s+pea|green\\s+pea|yellow\\s+pea|lupin|mung\\s+bean|fava\\s+bean|black\\s+bean|kidney\\s+bean|navy\\s+bean|pinto\\s+bean|lima\\s+bean|textured\\s+vegetable\\s+protein|tvp|soy\\s+lecithin|soy\\s+protein|peanut\\s+(butter|oil|flour))\\b" },
        { Ingredients.Gluten, "\\b(wheat|barley|rye|triticale|spelt|farro|kamut|khorasan\\s+wheat|durum|einkorn|semolina|bulgur|couscous|malt|seitan|gluten)\\b"},
        { Ingredients.Egg, "\\b(egg|eggs|egg\\s+yolk|egg\\s+white|dried\\s+egg|powdered\\s+egg|whole\\s+egg|albumen|egg\\s+albumen|ovomucoid|ovalbumin|lysozyme|meringue|mayonnaise)\\b" },
        { Ingredients.Fish, "\\b(fish|fish\\s+oil|fish\\s+protein|fish\\s+gelatin|fish\\s+sauce|fermented\\s+fish|dried\\s+fish|anchovy|anchovies|anchovy\\s+paste|tuna|salmon|cod|haddock|halibut|tilapia|pollock|bass|snapper|mackerel|sardine|trout|swordfish|flounder|sole)\\b"},
        { Ingredients.Sesame, "\\b(sesame|sesame\\s+seeds?|sesame\\s+oil|toasted\\s+sesame|sesame\\s+paste|sesame\\s+flour|ground\\s+sesame|tahini|gomashio|gomasio|benne\\s+seed|halvah|halva|simsim)\\b" },
        { Ingredients.Corn, "\\b(corn|maize|corn\\s+flour|cornmeal|corn\\s+starch|cornstarch|corn\\s+oil|corn\\s+syrup|high\\s+fructose\\s+corn\\s+syrup|hfcs|corn\\s+gluten|corn\\s+alcohol|corn\\s+syrup\\s+solids)\\b" },
        { Ingredients.Soy, "\\b(soy|soya|soybean|soybeans|soy\\s+flour|soy\\s+protein|soy\\s+protein\\s+isolate|soy\\s+protein\\s+concentrate|soy\\s+lecithin|soy\\s+sauce|miso|tempeh|tofu|natto|edamame|textured\\s+vegetable\\s+protein|tvp|hydrolyzed\\s+soy\\s+protein)\\b" },
        { Ingredients.Peanuts, "\\b(peanut|peanuts|groundnut|peanut\\s+butter|peanut\\s+oil|peanut\\s+flour|defatted\\s+peanut\\s+flour|peanut\\s+meal|peanut\\s+protein)\\b" },
        { Ingredients.ArtificialSweeteners, "\\b(aspartame|sucralose|saccharin|acesulfame\\s+k|acesulfame\\s+potassium|ace-k|neotame|advantame|cyclamate)\\b" },
        { Ingredients.Yeast, "\\b(yeast|baker'?s\\s+yeast|brewer'?s\\s+yeast|nutritional\\s+yeast|active\\s+dry\\s+yeast|instant\\s+yeast|fresh\\s+yeast|yeast\\s+extract|autolyzed\\s+yeast\\s+extract|hydrolyzed\\s+yeast\\s+extract|torula\\s+yeast|inactive\\s+yeast|yeast\\s+beta-?glucan)\\b" },
        { Ingredients.Nightshades, "\\b(tomato|tomatoes|potato|potatoes|eggplant|aubergine|bell\\s+pepper|chili\\s+pepper|chili|chilli|jalapeño|habanero|cayenne|paprika|pimento|goji\\s+berry|red\\s+pepper|crushed\\s+red\\s+pepper|chili\\s+powder|chilli\\s+powder)\\b"},
        { Ingredients.Meat, "\\b(beef|veal|lamb|mutton|venison|bison|pork|bacon|ham|prosciutto|pancetta|chorizo|sausage|chicken|turkey|duck|goose|liver|kidney|heart|offal|tripe|bone\\s+marrow|gizzards|animal\\s+fat|tallow|suet|bone\\s+broth|meat\\s+(broth|stock)|chicken\\s+stock|beef\\s+fat|chicken\\s+fat|duck\\s+fat)\\b" },
        { Ingredients.Gelatin, "\\b(gelatin|pork\\s+gelatin|beef\\s+gelatin|fish\\s+gelatin|hydrolyzed\\s+gelatin|collagen|hydrolyzed\\s+collagen|collagen\\s+peptides)\\b" },
        { Ingredients.Caffeine, "\\b(caffeine|caffeine\\s+anhydrous|caffeine\\s+extract|coffee|espresso|cold\\s+brew|green\\s+tea|black\\s+tea|matcha|oolong\\s+tea|yerba\\s+mate|guayusa|guarana|guarana\\s+extract|kola\\s+nut|theobromine|chocolate|dark\\s+chocolate|cocoa|cocoa\\s+powder)\\b" },
        { Ingredients.AddedSugar, "\\b(sugar|cane\\s+sugar|brown\\s+sugar|raw\\s+sugar|invert\\s+sugar|confectioner'?s\\s+sugar|granulated\\s+sugar|corn\\s+syrup|high\\s+fructose\\s+corn\\s+syrup|hfcs|glucose\\s+syrup|fructose\\s+syrup|malt\\s+syrup|rice\\s+syrup|maple\\s+syrup|agave\\s+(nectar|syrup)|golden\\s+syrup|date\\s+syrup|molasses|treacle|barley\\s+malt|dextrose|fructose|glucose|sucrose|maltose|lactose)\\b" },
        { Ingredients.Honey, "\\b(honey|raw\\s+honey|organic\\s+honey|manuka\\s+honey|clover\\s+honey|wildflower\\s+honey|honey\\s+powder|dehydrated\\s+honey|honey\\s+extract|honey\\s+flavor|honey\\s+crystals|royal\\s+jelly|bee\\s+pollen|propolis)\\b" },
        { Ingredients.Preservatives, "\\b(sodium\\s+sulfite|sodium\\s+bisulfite|sodium\\s+metabisulfite|potassium\\s+metabisulfite|sulfur\\s+dioxide|sulphites?|sulfites?|sodium\\s+nitrite|sodium\\s+nitrate|potassium\\s+nitrate|potassium\\s+nitrite|bha|bht|tbhq|edta|calcium\\s+disodium\\s+edta|benzoic\\s+acid|sodium\\s+benzoate|potassium\\s+benzoate|sorbic\\s+acid|potassium\\s+sorbate|propionic\\s+acid|sodium\\s+propionate|calcium\\s+propionate|natamycin|e2\\d{2})\\b" },
        { Ingredients.Colorants, "\\b(red\\s+3|red\\s+40|yellow\\s+5|yellow\\s+6|blue\\s+1|blue\\s+2|green\\s+3|green\\s+s|orange\\s+b|carmoisine|ponceau\\s+4r|allura\\s+red|brilliant\\s+blue|fast\\s+green|indigo\\s+carmine|erythrosine|e10\\d|e12\\d|e13\\d|e14\\d|e1[0-4][0-9])\\b" },
        { Ingredients.MSG, "\\b(msg|monosodium\\s+glutamate|glutamic\\s+acid|autolyzed\\s+yeast|hydrolyzed\\s+(protein|vegetable\\s+protein|soy\\s+protein)|sodium\\s+caseinate|calcium\\s+caseinate|yeast\\s+extract|textured\\s+protein|umami\\s+seasoning)\\b" },
        { Ingredients.Alcohol, "\\b(alcohol|ethyl\\s+alcohol|ethanol|spirits|liquor|beer|wine|red\\s+wine|white\\s+wine|cooking\\s+wine|sherry|sake|port|brandy|rum|vodka|gin|whiskey|whisky|bourbon|cider|vanilla\\s+extract|almond\\s+extract|rum\\s+flavor|liqueur\\s+flavor|amaretto\\s+extract)\\b" },
        { Ingredients.RedMeat, "\\b(beef|veal|lamb|mutton|venison|bison|goat|ox|roast\\s+beef|pastrami|meatballs|beef\\s+broth|beef\\s+stock|beef\\s+fat|bone\\s+marrow)\\b" },
        { Ingredients.Poultry, "\\b(chicken|turkey|duck|goose|hen|capon|quail|pheasant|chicken\\s+broth|chicken\\s+stock|chicken\\s+fat|duck\\s+fat|poultry\\s+fat)\\b" }
    };

    private static readonly Dictionary<FoodRestrictions, string> _foodRestrictions = new()
    {
        { FoodRestrictions.GlutenFree, "\\b(wheat|barley|rye|triticale|spelt|farro|kamut|khorasan\\s+wheat|durum|einkorn|semolina|bulgur|couscous|malt|seitan|gluten)\\b" },
        { FoodRestrictions.Vegan, "\\b(beef|pork|chicken|lamb|veal|duck|turkey|goose|bacon|ham|salami|sausage|prosciutto|meatballs?|cold\\s+cuts|lard|tallow|liver|kidney|heart|offal|suet|bone\\s+broth|meat\\s+(stock|flavor|extract)|animal\\s+fat|fish|anchovy|herring|flounder|scallops|oysters|trout|tuna|salmon|sockeye|tilapia|clam|sushi|haddock|cod|shrimp|crab|lobster|sardine(s)?|shellfish|gelatin|collagen|rennet|isinglass|carmine|cochineal|shellac|milk|cheese|butter|cream|whey|casein|yogurt|egg|eggs|albumen|lysozyme|honey|beeswax|royal\\s+jelly|propolis)\\b" },
        { FoodRestrictions.Vegetarian, "\\b(beef|pork|chicken|lamb|veal|duck|turkey|goose|bacon|ham|salami|sausage|prosciutto|meatballs?|cold\\s+cuts|lard|tallow|liver|kidney|heart|offal|suet|bone\\s+broth|meat\\s+(stock|flavor|extract)|animal\\s+fat|fish|anchovy|tuna|salmon|cod|shrimp|crab|lobster|sardine|shellfish|gelatin|collagen|rennet|isinglass|carmine|cochineal|shellac)\\b" },
        { FoodRestrictions.Halal, "\\b(pork|bacon|ham|prosciutto|lard|gelatin|alcohol|ethanol|beer|wine|brandy|rum|vodka|bourbon|blood|blood\\s+plasma|non-halal)\\b" },
        { FoodRestrictions.Kosher, "\\b(pork|bacon|ham|prosciutto|shellfish|shrimp|crab|lobster|clam|oyster|scallop|mussels|gelatin|rennet|blood|meat\\s+and\\s+dairy|wine|grape\\s+juice)\\b" },
        { FoodRestrictions.Pescatarian, "\\b(beef|pork|bacon|ham|prosciutto|lamb|veal|chicken|turkey|duck|goose|game|venison|lard|tallow|suet|bone\\s+broth|meat\\s+stock|animal\\s+fat|liver|kidney|heart|offal)\\b" },
        { FoodRestrictions.LactoseIntolerant, "\\b(milk|whole\\s+milk|skim\\s+milk|cream|condensed\\s+milk|evaporated\\s+milk|milk\\s+powder|milk\\s+solids|sour\\s+cream|buttermilk|lactose|whey|whey\\s+protein|whey\\s+protein\\s+concentrate|whey\\s+protein\\s+isolate|curds)\\b" }
    };

    public static string GetIngredientRegex(string ingredient)
    {
        var ingredientEnum = Enum.Parse(typeof(Ingredients), ingredient);

        return _ingredients[(Ingredients)ingredientEnum];
    }

    public static string GetFoodRestrictionRegex(string restriction)
    {
        var restrictionEnum = Enum.Parse(typeof(FoodRestrictions), restriction);

        return _foodRestrictions[(FoodRestrictions)restrictionEnum];
    }
}