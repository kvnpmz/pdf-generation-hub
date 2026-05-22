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
    const float H2Height = 14f;
    const float H2PaddingBottom = 2f;
    const float H2TotalHeight = H2Height + H2PaddingBottom;
    const float ItemHeight = 14f;
    const float ColumnPadding = 6f;
    const float ColumnBorder = 1.3f;
    const float ColumnSpacing = 4f;
    const int MaxChars = 33;
    const string fileName = "gallbladderdiet_list";


    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        Stopwatch sw = Stopwatch.StartNew();
        
        var column1Data = new DataColumn
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

        var column2Data = new DataColumn
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

        var column3Data = new DataColumn
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
                            .Text("Gallbladder Diet Food List".ToUpper())
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
                                        .PaddingTop(2.5f)
                                        .Width(4f)      
                                        .Height(4f)
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
    public float TitleFontSize { get; set; } = 30f;
    public float HeaderTableFontSize { get; set; } = 16f;
    public float SectionTitleFontSize { get; set; } = 11f;
    public float ItemFontSize { get; set; } = 9f;
    public string PrimaryColor = "#121212";
}
