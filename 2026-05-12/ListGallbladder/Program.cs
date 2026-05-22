using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO; // Required for Path.GetFullPath
using QuestPDF.Infrastructure;

class Program
{
    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        Stopwatch sw = Stopwatch.StartNew();

        var col1 = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Fruits", Items = new List<string> { "Apples (with skin)", "Pears", "Berries (strawberries, blueberries, raspberries)", "Oranges, grapefruit", "Papaya, melon", "Bananas (in moderation)" } },
                new DataSection { Title = "Vegetables", Items = new List<string> { "Leafy greens (spinach, lettuce, kale)", "Carrots", "Broccoli", "Zucchini", "Cauliflower", "Green beans", "Cucumber", "Pumpkin and squash" } },
                new DataSection { Title = "Grains & Starches", Items = new List<string> { "Oatmeal", "Brown rice", "Quinoa", "Whole wheat bread and pasta", "Barley", "Sweet potatoes (baked or boiled)" } },
                new DataSection { Title = "Protein (Low Fat)", Items = new List<string> { "Skinless chicken or turkey", "Fish (cod, tilapia, tuna)", "Egg whites", "Tofu", "Lentils and beans (well cooked)", "Low-fat cottage cheese" } },
                new DataSection { Title = "Dairy & Alternatives", Items = new List<string> { "Fat-free or low-fat milk", "Low-fat yogurt", "Unsweetened plant milk (almond, oat)" } },
                new DataSection { Title = "Others", Items = new List<string> { "Clear vegetable soups", "Homemade broth", "Herbs (parsley, basil, dill)", "Plenty of water and herbal teas" } }
            }
        };

        var col2 = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Protein Foods", Items = new List<string> { "Whole eggs (limit yolks)", "Lean red meat (sirloin, tenderloin)", "Fatty fish (salmon, mackerel)", "Ground poultry (lean only)", "Shrimp", "Low-fat deli meats (occasionally)" } },
                new DataSection { Title = "Healthy Fats", Items = new List<string> { "Avocado (1-2 slices)", "Olive oil (1/2-1 tsp per meal)", "Canola or sunflower oil", "Light mayonnaise", "Light salad dressings" } },
                new DataSection { Title = "Nuts, Seeds & Spreads", Items = new List<string> { "Almonds, walnuts, cashews (small handful)", "Chia seeds, flaxseeds", "Peanut butter or almond butter (thin spread)", "Tahini (very small amount)" } },
                new DataSection { Title = "Dairy & Alternatives", Items = new List<string> { "Reduced-fat cheese", "Low-fat cream cheese", "Low-fat sour cream", "Soy milk (unsweetened)", "Low-fat frozen yogurt" } },
                new DataSection { Title = "Grains & Baked Foods", Items = new List<string> { "White rice, White bread", "Refined pasta, Crackers", "Pancakes or waffles (plain, no butter)" } },
                new DataSection { Title = "Beverages", Items = new List<string> { "Coffee (1 small cup)", "Black or green tea", "Low-acid juices", "Diet sodas (occasional)" } },
                new DataSection { Title = "Seasonings & Extras", Items = new List<string> { "Mild spices", "Tomato sauce (small portions)", "Onions and garlic (cooked)", "Ketchup and BBQ sauce (limited)", "Pickled foods (small amounts)" } }
            }
        };

        var col3 = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "High-Fat Foods", Items = new List<string> { "Fried snacks", "Fried or breaded meat", "Fast food", "Greasy snacks", "Butter, ghee, margarine", "Full-fat cheese and cream", "Whipped cream and heavy cream", "Mayonnaise" } },
                new DataSection { Title = "Meat & Protein", Items = new List<string> { "Bacon", "Sausage", "Hot dogs", "Fatty beef or lamb", "Organ meats", "Salami, pepperoni, and bologna" } },
                new DataSection { Title = "Processed & Sugary Foods", Items = new List<string> { "Pastries, cakes, donuts", "Chocolate", "Candy", "Ice cream", "Packaged snacks", "Buttered popcorn" } },
                new DataSection { Title = "Refined Carbohydrates", Items = new List<string> { "White bread", "White pasta", "Sugary cereals", "Potato chips", "Corn chips", "Commercial crackers with trans fats" } },
                new DataSection { Title = "Spicy & Irritating Foods", Items = new List<string> { "Chili", "Hot sauces", "Heavy gravies", "Garlic-heavy or onion-heavy dishes" } },
                new DataSection { Title = "Beverages", Items = new List<string> { "Alcohol", "Energy drinks", "Sugary sodas" } }
            }
        };

        IPdfGeneratorStrategy strategy = new GallbladderDietStrategy();

        var theme = new ReportTheme();

        string fileName = "gallbladder_diet";

        var columns = new List<DataColumn> { col1, col2, col3 };

        strategy.Generate($"{fileName}_letter", 612f, 792f, columns, theme);
        strategy.Generate($"{fileName}_a4", 595f, 842f, columns, theme);

        sw.Stop();

        Console.WriteLine("--- Generation Complete ---");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_letter.pdf")}");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}_a4.pdf")}");
        Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds:F3}s");
    }
}
