using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuestPDF.Infrastructure;

class Program {
    static void Main() {
        QuestPDF.Settings.License = LicenseType.Community;
        Stopwatch sw = Stopwatch.StartNew();

	var col1 = new DataColumn
	{
	    Sections = new List<DataSection>
	    {
		new DataSection
		{
		    Title = "Lean Protein",
		    Items = new List<string> { "Chicken breast", "Turkey", "Eggs", "Tofu", "Greek yogurt", "Salmon", "Tuna", "Lentils", "Chickpeas" }
		},
		new DataSection
		{
		    Title = "High-Fiber Vegetables",
		    Items = new List<string> { "Broccoli", "Spinach", "Kale", "Zucchini", "Brussels sprouts", "Cabbage", "Cauliflower", "Asparagus" }
		},
		new DataSection
		{
		    Title = "Low-Sugar Fruits",
		    Items = new List<string> { "Berries (strawberries, blueberries, raspberries)", "Apples", "Pears", "Oranges", "Kiwi" }
		},
		new DataSection
		{
		    Title = "Whole Grains",
		    Items = new List<string> { "Oats", "Quinoa", "Brown rice", "Whole-grain bread", "Barley" }
		},
		new DataSection
		{
		    Title = "Healthy Fats",
		    Items = new List<string> { "Avocados", "Olive oil", "Almonds", "Walnuts", "Chia seeds", "Flaxseeds", "Nut butters (almond, peanut)", "Pistachios" }
		},
		new DataSection
		{
		    Title = "Hydration & Electrolytes",
		    Items = new List<string> { "Water with lemon/lime", "Bone broth", "Herbal tea (ginger, peppermint)", "Sugar-free electrolyte drinks" }
		}
	    }
	};
	
	var col2 = new DataColumn
	{
	    Sections = new List<DataSection>
	    {
		new DataSection
		{
		    Title = "Refined Carbohydrates",
		    Items = new List<string> { "White rice", "White pasta", "White bread", "Bagels", "Regular tortillas", "Pancakes and waffles" }
		},
		new DataSection
		{
		    Title = "Higher-Sugar Fruits",
		    Items = new List<string> { "Bananas", "Mango", "Pineapple", "Grapes", "Cherries", "Dried fruits (raisins, dates, dried mango)" }
		},
		new DataSection
		{
		    Title = "Starchy Vegetables",
		    Items = new List<string> { "White potatoes", "Sweet potatoes", "Corn", "Peas" }
		},
		new DataSection
		{
		    Title = "Dairy Products",
		    Items = new List<string> { "Whole milk", "Cream", "Flavored yogurt", "Sweetened milk drinks", "Ice cream" }
		},
		new DataSection
		{
		    Title = "Processed or Convenience Foods",
		    Items = new List<string> { "Crackers", "Granola with added sugar", "Breakfast cereals", "Packaged snack bars", "Instant noodles" }
		},
		new DataSection
		{
		    Title = "Fatty or Heavy Foods",
		    Items = new List<string> { "Fatty red meat", "Bacon", "Sausage", "Butter", "Cream-based sauces" }
		},
		new DataSection
		{
		    Title = "Sweet Foods",
		    Items = new List<string> { "Cakes", "Cookies", "Pastries", "Milk chocolate", "Sweet desserts" }
		}
	    }
	};
	
	var col3 = new DataColumn
	{
	    Sections = new List<DataSection>
	    {
		new DataSection
		{
		    Title = "Sugary Drinks",
		    Items = new List<string> { "Regular soda", "Sweetened fruit juices", "Energy drinks", "Sweetened iced tea", "Flavored coffee drinks with sugar", "Sports drinks with added sugar", "Milkshakes" }
		},
		new DataSection
		{
		    Title = "Highly Processed Sweets",
		    Items = new List<string> { "Candy bars", "Gummies", "Caramel sweets", "Chocolate with high sugar", "Donuts", "Cupcakes", "Frosted cakes", "Sweet pastries" }
		},
		new DataSection
		{
		    Title = "Refined Grain Products",
		    Items = new List<string> { "White bread", "White pasta", "White rice in large portions", "Sugary breakfast cereals", "Packaged muffins", "Sweet rolls" }
		},
		new DataSection
		{
		    Title = "Ultra-Processed Snack Foods",
		    Items = new List<string> { "Potato chips", "Corn chips", "Cheese puffs", "Flavored crackers", "Instant noodles", "Packaged snack cakes" }
		},
		new DataSection
		{
		    Title = "Deep-Fried Foods",
		    Items = new List<string> { "French fries", "Fried chicken", "Fried fish", "Fried fast-food items", "Fried appetizers" }
		},
		new DataSection
		{
		    Title = "Fast Food & Highly Processed Meals",
		    Items = new List<string> { "Burgers with refined buns", "Large fast-food combo meals", "Processed frozen dinners", "Breaded frozen foods", "Processed meat sandwiches" }
		},
		new DataSection
		{
		    Title = "Sugary Condiments & Add-Ons",
		    Items = new List<string> { "Sweet syrups", "Caramel sauces", "Sweet salad dressings", "Flavored sugar creamers", "Sweet spreads" }
		}
	    }
	};

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

	string fileName = "glp1diet_list";

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
