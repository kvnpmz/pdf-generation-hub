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
                new DataSection
                {
                    Title = "Vegetables",
                    Items = new List<string>
                    {
                        "Spinach", "Kale", "Swiss chard", "Collard greens", "Arugula",
                        "Romaine lettuce", "Butter lettuce", "Iceberg lettuce", "Cabbage",
                        "Bok choy", "Broccoli", "Broccolini", "Cauliflower", "Brussels sprouts",
                        "Asparagus", "Green beans", "Snow peas", "Sugar snap peas", "Zucchini",
                        "Yellow squash", "Eggplant", "Cucumber", "Celery", "Radishes",
                        "Turnips", "Okra", "Mushrooms", "Bell peppers", "Cherry tomatoes",
                        "Tomatoes", "Onion", "Garlic", "Leeks", "Green onions", "Fresh herbs"
                    }
                },
                new DataSection
                {
                    Title = "Pantry Staples",
                    Items = new List<string>
                    {
                        "Canned tuna", "Canned salmon", "Canned sardines",
                        "Canned beans", "Canned tomatoes", "Low-sodium"
                    }
                },
            }
        };

        var col2 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Protein - Meat & Poultry",
                    Items = new List<string>
                    {
                        "Chicken breast", "Chicken thighs", "Turkey breast", "Ground turkey",
                        "Lean ground beef", "Sirloin steak", "Tenderloin beef", "Bison",
                        "Lamb", "Duck", "Goat meat"
                    }
                },
                new DataSection
                {
                    Title = "Legumes (Slow Glucose Rise)",
                    Items = new List<string>
                    {
                        "Lentils", "Red lentils", "Green lentils", "Brown lentils",
                        "Chickpeas", "Black beans", "Kidney beans", "Cannellini beans",
                        "Pinto beans", "Navy beans", "Adzuki beans", "Lima beans"
                    }
                },
                new DataSection
                {
                    Title = "Healthy Fats & Oils",
                    Items = new List<string>
                    {
                        "Extra virgin olive oil", "Avocado oil", "Coconut oil",
                        "Ghee", "Grass-fed butter", "Tahini", "Nut butters (unsweetened)"
                    }
                },
                new DataSection
                {
                    Title = "Frozen",
                    Items = new List<string>
                    {
                        "Frozen berries", "Frozen spinach", "Frozen broccoli",
                        "Frozen cauliflower", "Frozen mixed"
                    }
                },
            }
        };

        var col3 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Starchy Veggies (Portion Controlled)",
                    Items = new List<string>
                    {
                        "Sweet potato", "Yam", "Butternut squash", "Acorn squash",
                        "Spaghetti squash", "Pumpkin", "Corn", "Green peas",
                        "Plantain", "Parsnips", "Beets", "Carrots"
                    }
                },
                new DataSection
                {
                    Title = "Whole Grains (Portion Controlled)",
                    Items = new List<string>
                    {
                        "Quinoa", "Steel-cut oats", "Rolled oats", "Barley",
                        "Bulgur", "Farro", "Brown rice", "Wild rice",
                        "Millet", "Sorghum", "Buckwheat", "Amaranth", "Teff"
                    }
                },
                new DataSection
                {
                    Title = "Condiments",
                    Items = new List<string>
                    {
                        "Apple cider vinegar", "Balsamic vinegar", "Red wine vinegar",
                        "Mustard", "Hummus", "Guacamole", "Salsa", "Low-sugar ketchup",
                        "Hot sauce", "Soy sauce", "Tamari", "Tahini"
                    }
                },
            }
        };

        var col4 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Eggs & Dairy",
                    Items = new List<string>
                    {
                        "Eggs", "Greek yogurt (plain)", "Skyr yogurt", "Cottage cheese",
                        "Ricotta cheese", "Mozzarella cheese", "Cheddar cheese",
                        "Feta cheese", "Parmesan cheese", "Kefir", "Unsweetened milk",
                        "Unsweetened almond milk", "Unsweetened soy milk"
                    }
                },
                new DataSection
                {
                    Title = "Low-Carb Snacks",
                    Items = new List<string>
                    {
                        "Mixed nuts", "Roasted chickpeas", "Pumpkin seeds", "Greek yogurt",
                        "Cottage cheese", "Boiled eggs", "Cheese sticks", "Celery with peanut butter",
                        "Apple with almond butter", "Hummus with vegetables"
                    }
                },
                new DataSection
                {
                    Title = "Seeds",
                    Items = new List<string>
                    {
                        "Chia seeds", "Flaxseeds", "Hemp seeds", "Pumpkin seeds",
                        "Sunflower seeds", "Sesame seeds", "Poppy seeds"
                    }
                },
                new DataSection
                {
                    Title = "Refrigerated",
                    Items = new List<string>
                    {
                        "Tofu", "Tempeh", "Hummus", "Guacamole"
                    }
                },
            }
        };

        var col5 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Low-Glycemic Fruits",
                    Items = new List<string>
                    {
                        "Blueberries", "Strawberries", "Raspberries", "Blackberries",
                        "Cherries", "Plums", "Peaches", "Apricots", "Apples",
                        "Pears", "Kiwi", "Oranges", "Grapefruit", "Pomegranate", "Guava"
                    }
                },
                new DataSection
                {
                    Title = "Seafood (Good for Diabetes)",
                    Items = new List<string>
                    {
                        "Salmon", "Sardines", "Mackerel", "Tuna", "Trout",
                        "Cod", "Halibut", "Tilapia", "Anchovies", "Shrimp",
                        "Prawns", "Scallops", "Mussels", "Clams", "Oysters"
                    }
                },
                new DataSection
                {
                    Title = "Baking & Sweeteners",
                    Items = new List<string>
                    {
                        "Almond flour", "Coconut flour", "Oat flour", "Ground flaxseed",
                        "Unsweetened cocoa powder", "Dark chocolate (85%+)", "Stevia"
                    }
                },
            }
        };

        var col6 = new DataColumn
        {
            Sections = new List<DataSection>
            {
                new DataSection
                {
                    Title = "Spices",
                    Items = new List<string>
                    {
                        "Cinnamon", "Turmeric", "Ginger", "Garlic powder",
                        "Onion powder", "Cumin", "Coriander", "Paprika",
                        "Black pepper", "Cayenne pepper", "Cardamom", "Cloves",
                        "Fenugreek", "Mustard powder"
                    }
                },
                new DataSection
                {
                    Title = "Nuts",
                    Items = new List<string>
                    {
                        "Almonds", "Walnuts", "Pistachios", "Cashews", "Macadamia nuts",
                        "Brazil nuts", "Pecans", "Pine nuts", "Hazelnuts"
                    }
                },
                new DataSection
                {
                    Title = "Beverages",
                    Items = new List<string>
                    {
                        "Water", "Mineral water", "Sparkling water", "Green tea",
                        "Black tea", "Herbal tea", "Chamomile tea", "Peppermint tea", "Unsweetened"
                    }
                },
                new DataSection
                {
                    Title = "Herbs",
                    Items = new List<string>
                    {
                        "Basil", "Parsley", "Cilantro", "Dill", "Mint",
                        "Thyme", "Rosemary", "Oregano", "Sage", "Chives"
                    }
                },
            }
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

        string fileName = "diabetes_groceries";

        var columns = new List<DataColumn> { col1, col2, col3, col4, col5, col6 };

        strategy.Generate($"{fileName}_letter", PageSize.Letter, columns, theme);
        strategy.Generate($"{fileName}_a4", PageSize.A4, columns, theme);

        sw.Stop();
        Console.WriteLine("--- Generation Complete ---");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_letter.pdf")}");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_a4.pdf")}");
        Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds:F3}s");
    }
}
