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
            new DataSection { Title = "", Items = new List<string> { "Red bell peppers", "Cauliflower & cabbage", "Green beans & celery", "Cucumber & radishes", "Arugula & lettuce", "Eggplant & zucchini", "Turnips & rutabaga", "White onion & garlic", "Watercress & fennel", "Leeks & bean sprouts", "Yellow squash & jicama", "Fresh/froz. blueberries", "Cranberries & grapes", "Raspberries & plums", "Pineapple & cherries", "Fresh/froz. strawberries", "Apple & pear slices", "Watermelon slices", "Fresh lemon juice", "Skinless chicken", "Fresh white fish", "Egg white omelets", "Lean ground turkey", "White rice & couscous", "Plain white bread", "Unsalted rice cakes", "Angel hair pasta", "Cream cheese spread", "Extra virgin olive oil", "Fresh ginger root", "Unsalted popcorn", "Turmeric & cinnamon", "Low-sodium broth", "Unsalted crackers", "Plain rice noodles", "Fresh basil & dill", "Unsalted macadamias", "Honey & fruit jams", "Rice milk & sherbet", "Herbal & rooibos tea", "Corn & flour tortillas", } }
            }
        };

        var col2 = new DataColumn
        {
            Sections = new List<DataSection>
            {
            new DataSection { Title = "", Items = new List<string> { 
"Whole milk & yogurt", "Cheddar & Swiss cheese", "Whole wheat bread", "Brown rice & quinoa", "Lean red meat cuts", "Whole eggs & yolks", "Bananas & oranges", "Kiwi & cantaloupe", "Mangoes & papayas", "Dried apricots & figs", "Baked & sweet potato", "Spinach & Swiss chard", "Tomato sauce & paste", "Broccoli & asparagus", "Cooked corn & peas", "Mushrooms & artichokes", "Black beans & lentils", "Chickpeas & hummus", "Natural peanut butter", "Almonds & walnuts", "Sunflower & chia seeds", "Coffee & black tea", "Dark chocolate squares", "Oatmeal & granola", "Firm tofu & edamame", "Soy milk & tempeh", "Canned tuna & salmon", "Beets & parsnips", "Orange & prune juice", "Pomegranate seeds", "Coconut milk & cream", "Raisins & dried dates", "Cottage & ricotta", "Pork & lamb chops", "Shellfish & shrimp", "Dark meat poultry", "Beer, wine & spirits", "Avocado & guacamole", "Bran cereal & muesli", "Sardines & anchovies", "Buttermilk & kefir" } }
            }
        };

        var col3 = new DataColumn
        {
            Sections = new List<DataSection>
            {
            new DataSection { Title = "", Items = new List<string> { 
"Star fruit & its juice", "Dark cola drinks", "Processed deli meats", "Canned soups & stews", "Instant ramen noodles", "Pickles & sauerkraut", "Salted chips & pretzels", "Soy sauce & teriyaki", "Frozen prepared meals", "Fast food burgers", "Hot dogs & sausages", "Bacon & cured ham", "Bologna & salami", "Salted canned goods", "Packaged mac & cheese", "Boxed stuffing mix", "Processed cheese slices", "Sweetened conden. milk", "Bottled salad dressing", "Bouillon cubes & powder", "Ketchup & BBQ sauce", "Canned baked beans", "Frozen pizza & pockets", "Microwave popcorn", "Jerky & dried meats", "Smoked fish & meats", "Canned chili & ravioli", "Cheese puffs & curls", "Salted mixed nuts", "Worcestershire sauce", "Canned biscuit dough", "Plantain & banana chips", "Energy & sports drinks", "Pancake & waffle mix", "Instant mashed potato", "Packaged snack cakes", "Frozen breaded fish", "Instant oatmeal packs", "Canned meat spreads", "Gravy & sauce packets", "Seasoning salt blends" } }
            }
        };

        IPdfGeneratorStrategy strategy = new LayoutStrategy();
        var theme = new ReportTheme();

        string fileHeader = "Kidney Disease (CKD)  Food List";
        string fileName = "kidney_diet";

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
