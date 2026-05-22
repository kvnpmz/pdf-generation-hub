using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

class Program
{
    //static bool DebugLayout = false;

    const float VerticalMargin = 36f;
    const float HorizontalMargin = 50.4f;
    const float H1Height = 36f;
    const float H1Margin = 6f;
    const float HeaderTable = 24f;
    const float H2Height = 11f;
    const float H2PaddingBottom = 2f;
    const float H2TotalHeight = H2Height + H2PaddingBottom;
    const float ItemHeight = 10f;
    const float ColumnPadding = 6f;
    const float ColumnBorder = 1.3f;
    const float ColumnSpacing = 4f;
    const int MaxChars = 35;
    const string fileName = "mediterranean_list";


    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        Stopwatch sw = Stopwatch.StartNew();
        
	var column1Data = new DataColumn
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
	
	var column2Data = new DataColumn
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
	
	var column3Data = new DataColumn
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

        var theme = new ReportTheme();

        GenerateFile($"{fileName}_letter", 612f, 792f, column1Data, column2Data, column3Data, theme);

        GenerateFile($"{fileName}_a4", 595f, 842f, column1Data, column2Data, column3Data, theme);

        sw.Stop();

        Console.WriteLine($"Done! Generated Letter and A4 in {sw.Elapsed.TotalSeconds:F3}s");
    }

    static void GenerateFile(string fileName, float width, float height, DataColumn col1, DataColumn col2, DataColumn col3, ReportTheme theme)
    {
        float internalOffset = (ColumnPadding * 2) + (ColumnBorder * 2);

        float availableContainerHeight = height 
                                        - (VerticalMargin * 2) 
                                        - H1Height 
                                        - H1Margin 
                                        - HeaderTable 
                                        - internalOffset
                                        - 2f;
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.DefaultTextStyle(x => x.FontFamily(theme.FontFamily));
                page.Size(width, height);
                page.MarginHorizontal(HorizontalMargin);
                page.MarginVertical(VerticalMargin);
                page.Content().Column(rootCol =>
                {
                    rootCol.Item().Height(H1Height).Layers(layers =>
                    {
                        layers.PrimaryLayer()
                            .AlignTop()
                            .Text("Mediterranean Diet Food List".ToUpper())
                            .Style(TextStyle
                                .Default
                                .FontSize(theme.TitleFontSize)
                                .Weight(FontWeight.ExtraBold)
                                .LetterSpacing(.04f)
                                .FontColor("#06bdf8"))
                            .AlignCenter();

                        layers.Layer()
                            .AlignBottom()
                            .PaddingBottom(3f)
                            .LineHorizontal(1.5f)
                            .LineColor(theme.PrimaryColor);
                    });
                    
                    rootCol.Item()
                        .PaddingTop(H1Margin * 1 / 3)
                        .PaddingBottom(H1Margin * 2 / 3)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().PaddingRight(ColumnSpacing / 2)
                                .Element(c => BlockStyle(c, "#b3deb6", "#2A7F2A", theme))
                                .Text("EAT".ToUpper())
                                .FontColor("#2A7F2A");

                            table.Cell().PaddingHorizontal(ColumnSpacing / 2)
                                .Element(c => BlockStyle(c, "#fff79b", "#795901", theme))
                                .Text("LIMIT".ToUpper())
                                .FontColor("#795901");

                            table.Cell().PaddingLeft(ColumnSpacing / 2)
                                .Element(c => BlockStyle(c, "#ff9fad", "#B22222", theme))
                                .Text("AVOID".ToUpper())
                                .FontColor("#B22222");
                        });

                    rootCol.Item().Row(row =>
                    {
                    row.Spacing(ColumnSpacing);

                    row.RelativeItem()
                        .CornerRadius(10f)
                        .Border(ColumnBorder)
                        .BorderColor(theme.PrimaryColor)
                        .Background("#E8F5E9")
                        .Padding(ColumnPadding)
                        .Element(c => BuildJustifiedColumn(c, availableContainerHeight, col1, theme));

                    row.RelativeItem()
                        .CornerRadius(10f)
                        .Border(ColumnBorder)
                        .BorderColor(Colors.Black)
                        .Background("#FFFDE7")
                        .Padding(ColumnPadding)
                        .Element(c => BuildJustifiedColumn(c, availableContainerHeight, col2, theme));

                    row.RelativeItem()
                        .CornerRadius(10f)
                        .Border(ColumnBorder)
                        .BorderColor(Colors.Black)
                        .Background("#FFEBEE")
                        .Padding(ColumnPadding)
                        .Element(c => BuildJustifiedColumn(c, availableContainerHeight, col3, theme));
                    });
                });
            });
        });
        
        doc.GeneratePdf($"{fileName}.pdf");

        //Process.Start("xdg-open", $"{fileName}.pdf");

        Console.WriteLine("--- Generation Complete ---");
        Console.WriteLine($"File saved to: {Path.GetFullPath($"{fileName}.pdf")}");
        }

        static IContainer BlockStyle(IContainer container, string backgroundColor, string borderColor, ReportTheme theme)
        {
        return container
            .Height(HeaderTable)
            .CornerRadius(10f)
            .Background(backgroundColor)
            .Border(1.3f)
            .BorderColor(borderColor)
            .AlignCenter()
            .AlignMiddle()
            .DefaultTextStyle(x => x.FontSize(theme.HeaderTableFontSize).Bold());
        }

        static void BuildJustifiedColumn(IContainer container, float totalHeight, DataColumn data, ReportTheme theme)
        {

        int totalSections = data.Sections.Count;
        int actualLineCount = 0;
        foreach (var s in data.Sections)
        {
            foreach (var item in s.Items)
            {
                var lines = SplitText(item, MaxChars);

                actualLineCount += lines.Count;
            }
        }

        float usedSpace =
            (totalSections * (H2TotalHeight)) +
            (actualLineCount * ItemHeight);
        float leftoverSpace = totalHeight - usedSpace;
        float spacerHeight = totalSections > 1 ? (leftoverSpace / (totalSections - 1)) : 0;
        spacerHeight = Math.Max(0, spacerHeight);

        container.Column(col =>
        {
            for (int i = 0; i < totalSections; i++)
            {
                var section = data.Sections[i];
                col.Item().Column(inner =>
                {
                    inner.Item().Height(H2TotalHeight).Layers(layers =>
                    {
                        layers.PrimaryLayer()
                            //.PaddingBottom(H2PaddingBottom)
                            .AlignTop()
                            .Text(section.Title)
                            .Bold()
                            .FontSize(theme.SectionTitleFontSize);

                        layers.Layer()
                            .PaddingBottom(H2PaddingBottom)
                            .AlignBottom()
                            .LineHorizontal(1f)
                            .LineColor(theme.PrimaryColor);
                    });

                    foreach (var item in section.Items)
                    {
                        var lines = SplitText(item, MaxChars);
                        for (int j = 0; j < lines.Count; j++)
                        {
                            // Replace your existing inner.Item().Height(ItemHeight)... block with this:
                            inner.Item().Height(ItemHeight).Layers(layers =>
                            {
                                // 1. The Bullet (Pseudo-element)
                                if (j == 0) // Only draw bullet on the first line of a wrapped item
                                {
                                    layers.Layer()
                                        .AlignLeft()
                                        .PaddingTop(3.5f)
                                        .Width(3f)      
                                        .Height(3f)
                                        .Background(theme.PrimaryColor)
                                        .CornerRadius(2.25f);
                                }

                                // 2. The Text (Primary Content)
                                layers.PrimaryLayer()
                                    .PaddingLeft(6f)
                                    .Text(lines[j])
                                    .Bold()
                                    .FontSize(theme.ItemFontSize);
                            });
                        }
                    }
                });

                if (i < totalSections - 1)
                {
                    col.Item().Height(spacerHeight);
                }
            }
        });
    }

    static List<string> SplitText(string text, int maxChars)
    {
    var lines = new List<string>();

    if (string.IsNullOrEmpty(text)) return lines;
    if (text.Length <= maxChars) 
    {
        lines.Add(text);
        return lines;
    }

    string remainingText = text;

    while (remainingText.Length > maxChars)
    {
        int lastSpace = remainingText.LastIndexOf(' ', maxChars);

        int breakIndex = (lastSpace == -1) ? maxChars : lastSpace;

        lines.Add(remainingText.Substring(0, breakIndex).Trim());
        remainingText = remainingText.Substring(breakIndex).Trim();
    }

    if (!string.IsNullOrWhiteSpace(remainingText))
        lines.Add(remainingText);

    return lines;
    }
}

public class DataSection { public string Title { get; set; } = ""; public List<string> Items { get; set; } = new(); }
public class DataColumn { public List<DataSection> Sections { get; set; } = new(); }
public class ReportTheme
{
    public string FontFamily { get; set; } = "Ubuntu Sans"; 
    public float TitleFontSize { get; set; } = 27.5f;
    public float HeaderTableFontSize { get; set; } = 16f;
    public float SectionTitleFontSize { get; set; } = 9f;
    public float ItemFontSize { get; set; } = 8f;
    public string PrimaryColor = "#121212";
}
