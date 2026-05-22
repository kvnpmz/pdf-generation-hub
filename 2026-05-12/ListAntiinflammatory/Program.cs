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
		    Title = "",
		    Items = new List<string>
		    {
			"Salmon & Cod",
			"Kale & Chard",
			"Chia & Flax",
			"Apple & Pear",
			"Garlic & Onion",
			"Walnut & Pecan",
			"Olive Oil",
			"Turmeric & Ginger",
			"Berry & Plum",
			"Oats & Quinoa",
			"Tomato & Pepper",
			"Almond & Macadamia",
			"Broccoli & Cabbage",
			"Lentil & Bean",
			"Cherry & Grapefruit",
			"Spinach & Arugula",
			"Sweet Potato",
			"Pumpkin & Squash",
			"Cinnamon & Nutmeg",
			"Sardine & Mackerel",
			"Artichoke & Fennel",
			"Carrot & Radish",
			"Mushroom & Truffle",
			"Pomegranate",
			"Edamame & Tempeh",
			"Rosemary & Thyme",
			"Brussels Sprouts",
			"Orange & Lemon",
			"Green Tea & Matcha",
			"Cauliflower",
			"Basil & Mint",
			"Hemp Hearts"
		    }
		}
	    }
	};
	
	var col2 = new DataColumn
	{
	    Sections = new List<DataSection>
	    {
		new DataSection
		{
		    Title = "",
		    Items = new List<string>
		    {
			"Chicken & Duck",
			"Beef & Pork",
			"Milk & Yogurt",
			"Cheese & Butter",
			"Eggs & Mayonnaise",
			"White Rice & Pasta",
			"Potato & Corn",
			"Sourdough & Pita",
			"Coconut Oil",
			"Peanut & Cashew",
			"Honey & Maple",
			"Soy Sauce & Tamari",
			"Grape & Mango",
			"Coffee & Black Tea",
			"Wine & Champagne",
			"Canola Oil",
			"Couscous & Polenta",
			"Tortilla & Wrap",
			"Dark Chocolate",
			"Sour Cream",
			"Venison & Bison",
			"Lamb & Goat",
			"Oysters & Mussels",
			"Shrimp & Lobster",
			"Cottage Cheese",
			"White Flour & Bread",
			"Cornmeal & Grits",
			"Jam & Jelly",
			"Pineapple & Papaya",
			"Watermelon & Dates",
			"Ketchup & Mustard",
			"Pickles & Relish"
		    }
		}
	    }
	};
	
	var col3 = new DataColumn
	{
	    Sections = new List<DataSection>
	    {
		new DataSection
		{
		    Title = "",
		    Items = new List<string>
		    {
			"Bacon & Ham",
			"Hot Dog & Sausage",
			"Soda & Cola",
			"Margarine & Lard",
			"Shortening & Tallow",
			"Cake & Pie",
			"Candy & Gum",
			"Ice Cream & Gelato",
			"Fries & Tater Tots",
			"Onion Rings",
			"Fructose & Glucose",
			"Pizza & Calzones",
			"Energy Drink & Beer",
			"Liquor & Cocktails",
			"Fruit Juice",
			"Frosting & Icing",
			"Cereal & Puffs",
			"Brownies & Fudge",
			"Nachos & Pretzels",
			"Waffles & Pancakes",
			"Croutons & Crackers",
			"Bologna & Pastrami",
			"Soybean & Rapeseed",
			"Palm & Seed Oils",
			"Bouillon & MSG",
			"Ramen & Canned Soup",
			"White Chocolate",
			"Donuts & Pastries",
			"Pop Tarts & Strudel",
			"Canned Fruit",
			"Croissants & Scones",
			"Marshmallow, Nougat"
		    }
		}
	    }
	};

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

	string fileName = "antiinflamatory_list";

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
