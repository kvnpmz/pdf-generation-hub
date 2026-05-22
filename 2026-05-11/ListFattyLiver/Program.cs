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
    const float H2Height = 12f;
    const float H2PaddingBottom = 3f;
    const float H2TotalHeight = H2Height + H2PaddingBottom;
    const float ItemHeight = 11f;
    const float ColumnPadding = 6f;
    const float ColumnBorder = 1.3f;
    const float ColumnSpacing = 4f;
    const int MaxTitleChars = 35;
    const int MaxItemChars = 37;
    const string fileName = "fattyliver_list";


    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        Stopwatch sw = Stopwatch.StartNew();
        
        var column1Data = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Vegetables (Daily - Unlimited Variety)", Items = new List<string> { "Spinach, Broccoli", "Kale, Carrots", "Beetroot, Cabbage", "Cauliflower", "Brussels sprouts", "Zucchini", "Bell peppers, Asparagus", "Garlic (boosts liver enzymes)", "Onions, Cucumber", "Leafy greens (all types)" } },
                new DataSection { Title = "Fruits", Items = new List<string> { "Apples", "Berries (blueberries, strawberries, raspberries)", "Oranges, Grapefruit", "Lemon & lime, Pomegranate", "Papaya & Kiwi", "Pears", "Watermelon (small portions)" } },
                new DataSection { Title = "Whole Grains (High Fiber Choices)", Items = new List<string> { "Oats, Brown rice", "Quinoa", "Whole wheat", "Barley, Millet", "Buckwheat", "Whole grain bread (no added sugar)" } },
                new DataSection { Title = "Lean Protein (Supports Repair)", Items = new List<string> { "Chicken breast & Turkey", "Fish (salmon, sardines, mackerel)", "Eggs (boiled)", "Lentils, Chickpeas", "Beans (black beans, kidney beans)", "Tofu, Low-fat yogurt", "Cottage cheese (low-fat)" } },
                new DataSection { Title = "Healthy Fats (Reduce Inflammation)", Items = new List<string> { "Avocado", "Olive oil (extra virgin)", "Nuts (almonds, walnuts, pistachios)", "Seeds (chia, flax, sunflower, pumpkin)", "Fatty fish (omega-3 rich)" } },
                new DataSection { Title = "Drinks (Hydration + Detox Support)", Items = new List<string> { "Green tea, Black coffee (no sugar)", "Lemon water", "Herbal teas (ginger, turmeric, chamomile)", "Coconut water (unsweetened)", "Plenty of water (8-10 glasses daily)" } }
            }
        };

        var column2Data = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Refined Carbohydrates (Low Fiber)", Items = new List<string> { "White bread", "White rice", "Pasta (regular)", "Noodles", "Breakfast cereals (refined)", "Crackers", "Pancakes & waffles" } },
                new DataSection { Title = "High-Fat Dairy (Use Sparingly)", Items = new List<string> { "Butter", "Cheese (especially processed)", "Cream", "Full-fat milk", "Ice cream", "Flavored yogurt (high sugar)" } },
                new DataSection { Title = "Natural Sugars (Small Amounts Only)", Items = new List<string> { "Honey, Maple syrup", "Fruit juices (even fresh)", "Smoothies with added sugar", "Dried fruits (raisins, dates - very small portions)" } },
                new DataSection { Title = "Red Meat (Limit Weekly)", Items = new List<string> { "Beef, Lamb", "Goat meat, Pork" } },
                new DataSection { Title = "High Sodium Foods", Items = new List<string> { "Packaged soups", "Sauces (soy sauce, ketchup)", "Pickles", "Instant noodles", "Salted snacks" } },
                new DataSection { Title = "Caffeinated / Add-On Drinks", Items = new List<string> { "Coffee with sugar/cream", "Flavored lattes", "Energy drinks", "Sweetened tea" } },
                new DataSection { Title = "Processed \"Healthy\" Foods (Hidden Risks)", Items = new List<string> { "Granola bars", "Protein bars (added sugar)", "Low-fat packaged foods (often high sugar)" } }
            }
        };

        column2Data.Sections.Last().MaxTitleChars = 30;

        var column3Data = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Fried & Fast Foods", Items = new List<string> { "French fries, Fried chicken", "Burgers", "Pizza (greasy, processed cheese)", "Fried snacks (samosa, pakora, tempura)", "Onion rings, Fried street food" } },
                new DataSection { Title = "Sugar & Sweetened Foods", Items = new List<string> { "Soft drinks / soda", "Packaged fruit juices", "Candy, toffees", "Cakes, pastries, donuts", "Ice cream, Milk chocolate", "Syrups (corn syrup, glucose syrup)" } },
                new DataSection { Title = "Alcohol", Items = new List<string> { "Beer, Wine, Whiskey", "Vodka, Rum", "Cocktails" } },
                new DataSection { Title = "Processed Meats", Items = new List<string> { "Sausages, Bacon", "Salami, Hot dogs", "Pepperoni, Deli meats" } },
                new DataSection { Title = "Ultra-Processed Foods", Items = new List<string> { "Instant noodles", "Frozen ready meals", "Packaged soups", "Frozen fried items", "Boxed meal kits" } },
                new DataSection { Title = "High Sodium Foods", Items = new List<string> { "Chips & salted snacks", "Instant soups", "Pickles", "Sauces (soy sauce, ketchup)", "Canned processed foods" } },
                new DataSection { Title = "Unhealthy Fats & Oils", Items = new List<string> { "Palm oil", "Refined vegetable oils (overused)", "Reused frying oil", "Excess butter" } },
                new DataSection { Title = "Refined Carbohydrates", Items = new List<string> { "White bread", "White rice (large amounts)", "Pasta (refined flour)", "Bakery items" } },
                new DataSection { Title = "Sugary & Artificial Drinks", Items = new List<string> { "Sweetened coffee", "Flavored lattes", "Bubble tea", "Packaged iced tea", "Artificial fruit drinks" } }
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
                            .Text("Fatty Liver Food List".ToUpper())
                            .Style(TextStyle
                                .Default
                                .FontSize(theme.TitleFontSize)
                                .Weight(FontWeight.ExtraBold)
                                .LetterSpacing(.07f)
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
                                .Text("Eat".ToUpper())
                                .FontColor("#2A7F2A");

                            table.Cell().PaddingHorizontal(ColumnSpacing / 2)
                                .Element(c => BlockStyle(c, "#fff79b", "#795901", theme))
                                .Text("Limit".ToUpper())
                                .FontColor("#795901");

                            table.Cell().PaddingLeft(ColumnSpacing / 2)
                                .Element(c => BlockStyle(c, "#ff9fad", "#B22222", theme))
                                .Text("Avoid".ToUpper())
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
        int actualTitleLineCount = 0;

        foreach (var s in data.Sections)
        {
            var titleLines = SplitText(s.Title, s.MaxTitleChars);

            actualTitleLineCount += titleLines.Count;

            if (titleLines.Count > 1)
            {
                Console.WriteLine($"SPLIT TITLE -> \"{s.Title}\"");
                Console.WriteLine($"  Lines: {titleLines.Count}");
            }

            foreach (var item in s.Items)
            {
                actualLineCount += SplitText(item, MaxItemChars).Count;
            }
        }

        float usedSpace = 
            (actualTitleLineCount * H2Height)
            + (totalSections * H2PaddingBottom)
            + (actualLineCount * ItemHeight);
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
                    var titleLines = SplitText(section.Title, section.MaxTitleChars);

                    inner.Item().Column(titleCol =>
                    {
                        for (int t = 0; t < titleLines.Count; t++)
                        {
                            bool isLastTitleLine = (t == titleLines.Count - 1);
                            var lineText = titleLines[t];

                            var lineContainer = titleCol.Item().Height(isLastTitleLine ? H2TotalHeight : H2Height);

                            if (isLastTitleLine)
                            {
                                lineContainer.Layers(layers =>
                                {
                                    layers.PrimaryLayer()
                                        .AlignTop()
                                        .Text(lineText)
                                        .FontSize(theme.SectionTitleFontSize)
                                        .Bold()
                                        .FontColor(theme.PrimaryColor);

                                    layers.Layer()
                                        .AlignBottom()
                                        .PaddingBottom(H2PaddingBottom) 
                                        .LineHorizontal(1f)
                                        .LineColor(theme.PrimaryColor);
                                });
                            }
                            else
                            {
                                lineContainer.Text(lineText)
                                    .FontSize(theme.SectionTitleFontSize)
                                    .Bold()
                                    .FontColor(theme.PrimaryColor);
                            }
                        }
                    });

                    foreach (var item in section.Items)
                    {
                        var lines = SplitText(item, MaxItemChars);
                        for (int j = 0; j < lines.Count; j++)
                        {
                            inner.Item().Height(ItemHeight).Layers(layers =>
                            {
                                if (j == 0)
                                {
                                    layers.Layer()
                                        .AlignLeft()
                                        .PaddingTop(2.5f)
                                        .Width(4f)      
                                        .Height(4f)
                                        .Background(theme.PrimaryColor)
                                        .CornerRadius(2.25f);
                                }

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

public class DataSection {
    public string Title { get; set; } = "";
    public List<string> Items { get; set; } = new();
    public int MaxTitleChars { get; set; } = 35; // Default value
}
public class DataColumn {
    public List<DataSection> Sections { get; set; } = new();
}
public class ReportTheme
{
    public string FontFamily { get; set; } = "Ubuntu Sans"; 
    public float TitleFontSize { get; set; } = 30f;
    public float HeaderTableFontSize { get; set; } = 16f;
    public float SectionTitleFontSize { get; set; } = 8.5f;
    public float ItemFontSize { get; set; } = 8f;
    public string PrimaryColor = "#121212";
}
