using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuestPDF.Infrastructure;

class Program
{
    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        Stopwatch sw = Stopwatch.StartNew();

        var col1 = new DataColumn
        {
            Sections = new List<DataSection>
            {
            new DataSection { Title = "Vegetables", Items = new List<string> { "Leafy greens (spinach, kale, arugula, Swiss chard, collard greens)", "Tomatoes, cucumbers, bell peppers", "Zucchini, eggplant, broccoli, cauliflower", "Onions, mushrooms, carrots", "Artichokes, asparagus, fennel, okra", "Beets, green beans, sweet potatoes" } },
            new DataSection { Title = "Fruits", Items = new List<string> { "Berries, apples, oranges, grapes", "Figs, dates, pears, peaches", "Pomegranates, cherries", "Apricots, plums, melons, tangerines", "Mediterranean citrus (lemons, mandarins)" } },
            new DataSection { Title = "Whole Grains", Items = new List<string> { "Oats, quinoa, bulgur", "Brown rice, barley", "Whole-grain bread & pasta", "Farro, couscous, millet, sorghum", "Freekeh (cracked wheat grain)" } },
            new DataSection { Title = "Healthy Fats", Items = new List<string> { "Extra virgin olive oil (main fat)", "Avocado", "Olives", "Tahini (sesame paste)", "Olive tapenade" } },
            new DataSection { Title = "Legumes & Beans", Items = new List<string> { "Lentils, chickpeas, black beans", "Kidney beans, white beans, peas", "Fava beans (broad beans)", "Cannellini beans", "Split peas" } },
            new DataSection { Title = "Nuts & Seeds", Items = new List<string> { "Almonds, walnuts, pistachios", "Sunflower seeds, chia, flax", "Sesame seeds, pine nuts, pumpkin seeds", "Hazelnuts, cashews" } },
            new DataSection { Title = "Herbs & Spices", Items = new List<string> { "Basil, oregano, rosemary", "Garlic, mint, cumin, turmeric", "Thyme, parsley, dill", "Paprika, coriander, za’atar", "Saffron, sumac" } },
            new DataSection { Title = "Seafood (2-4 times/week)", Items = new List<string> { "Salmon, sardines, mackerel", "Tuna, trout", "Shrimp, prawns, mussels", "Anchovies, calamari, clams", "Sea bass, sea bream (Mediterranean staples)" } },
            new DataSection { Title = "Dairy (Moderate)", Items = new List<string> { "Greek yogurt", "Feta, mozzarella, ricotta", "Kefir" } }
            }
        };
        
        var col2 = new DataColumn
        {
            Sections = new List<DataSection>
            {
            new DataSection { Title = "Poultry", Items = new List<string> { "Chicken breast, thighs, wings", "Turkey breast, ground turkey", "Grilled or baked preferred", "Avoid fried or breaded versions" } },
            new DataSection { Title = "Eggs", Items = new List<string> { "Whole eggs", "Omelets and scrambled eggs", "Egg-based dishes (quiche, frittata)", "Consume in moderation (2–4/week)" } },
            new DataSection { Title = "Higher-Fat Dairy", Items = new List<string> { "Full-fat cheese (cheddar, gouda, brie)", "Cream cheese", "Sour cream", "Heavy cream", "Whole milk", "Butter (only tiny amounts recommended)" } },
            new DataSection { Title = "Red Meat", Items = new List<string> { "Beef, lamb, goat, veal", "Steak, burgers, meatballs", "Limit to small portions 1-2 times per month", "Choose lean cuts when possible" } },
            new DataSection { Title = "Refined Grains (Use sparingly)", Items = new List<string> { "White rice", "White pasta", "White bread", "Regular couscous", "Instant noodles", "Refined flour tortillas" } },
            new DataSection { Title = "Starchy Foods (Moderation)", Items = new List<string> { "Potatoes (fried or mashed with cream)", "Potato chips (limit strictly)", "Corn-based processed foods" } },
            new DataSection { Title = "Sweets & Desserts", Items = new List<string> { "Cakes, cupcakes", "Ice cream", "Cookies, brownies", "Pastries, donuts", "Muffins", "Sweetened yogurts", "Limit added sugar overall" } },
            new DataSection { Title = "Alcohol", Items = new List<string> { "Red wine optional but limited (1 glass/day or less)", "Beer or cocktails only occasionally", "Avoid binge drinking" } },
            new DataSection { Title = "Packaged Snacks (Limit quantity)", Items = new List<string> { "Crackers", "Pretzels", "Granola bars (with added sugar)", "Lightly processed nut mixes" } }
            }
        };
        
        var col3 = new DataColumn
        {
            Sections = new List<DataSection>
            {
            new DataSection { Title = "Processed Foods", Items = new List<string> { "Fast food (burgers, fries, fried chicken)", "Frozen meals with additives", "Microwave dinners", "Instant ramen" } },
            new DataSection { Title = "Highly Processed Meats", Items = new List<string> { "Bacon", "Sausage", "Pepperoni, salami", "Hot dogs", "Deli meats (ham, bologna, turkey slices)" } },
            new DataSection { Title = "Refined & Added Sugars", Items = new List<string> { "Sugary drinks (soda, energy drinks)", "Artificial fruit juices", "Candy & chocolate bars", "Breakfast cereals with added sugar", "Syrups (corn syrup, pancake syrup)", "Sweetened coffee drinks" } },
            new DataSection { Title = "Unhealthy Fats & Oils", Items = new List<string> { "Margarine", "Vegetable shortening", "Hydrogenated oils", "Industrial seed oils (corn, soybean, canola)", "Deep-fried foods" } },
            new DataSection { Title = "Ultra-Refined Carbs", Items = new List<string> { "White flour pastries", "Donuts", "Croissants", "White bread with added sugar", "Commercial bakery items" } },
            new DataSection { Title = "Processed Snacks", Items = new List<string> { "Chips (potato, tortilla)", "Cheese puffs", "Candy-coated nuts", "Sweetened popcorn" } },
            new DataSection { Title = "Fast-Food Desserts", Items = new List<string> { "Milkshakes", "Sundaes", "Fried desserts (churros, funnel cake)" } },
            new DataSection { Title = "Artificial & Chemical Additives", Items = new List<string> { "Artificial sweeteners (aspartame, sucralose)", "Artificial colorings", "Preservatives (BHA, BHT)", "Flavor enhancers (MSG, artificial flavors)" } },
            new DataSection { Title = "Excessive Salt Foods", Items = new List<string> { "Cup noodles", "Canned soups with high sodium", "Salted crackers, salted nuts", "Pretzels" } }
            }
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

        string fileHeader = "Mediterranean Diet Food List";
        string fileName = "mediterranean_diet";

        var columns = new List<DataColumn> { col1, col2, col3};

        var pageConfigurations = new[]
        {
            new { Suffix = "letter", Size = PageSize.Letter },
            new { Suffix = "a4", Size = PageSize.A4 }
        };

        foreach (var config in pageConfigurations)
        {
            strategy.Generate($"{fileName}_{config.Suffix}", fileHeader, config.Size, columns, theme);
        }

        sw.Stop();
        Console.WriteLine("--- Generation Complete ---");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_letter.pdf")}");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_a4.pdf")}");
        Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds:F3}s");
    }
}
