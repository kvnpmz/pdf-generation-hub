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
    const float H2PaddingBottom = 3f;
    const float H2TotalHeight = H2Height + H2PaddingBottom;
    const float ItemHeight = 14f;
    const float ColumnPadding = 6f;
    const float ColumnBorder = 1.3f;
    const float ColumnSpacing = 4f;
    const int MaxTitleChars = 26;
    const int MaxItemChars = 26;
    const string fileName = "renaldiet_list";


    static void Main()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        Stopwatch sw = Stopwatch.StartNew();
        
        var column1Data = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Grains & Starches", Items = new List<string> { "White rice", "White pasta", "White bread, rolls, naan", "Rice cereal, cornflakes (low sodium)", "Refined flour products" } },
                new DataSection { Title = "Proteins", Items = new List<string> { "Egg whites", "Small portions of plant protein as advised", "Homemade protein dishes with no added salt" } },
                new DataSection { Title = "Fruits (Low Potassium)", Items = new List<string> { "Apples, apple slices", "Grapes", "Blueberries, strawberries", "Pineapple", "Pears" } },
                new DataSection { Title = "Vegetables (Low Potassium)", Items = new List<string> { "Cabbage", "Cauliflower", "Cucumber", "Lettuce", "Zucchini", "Bell peppers", "Onions", "Green beans" } },
                new DataSection { Title = "Fats & Flavorings", Items = new List<string> { "Olive oil, vegetable oil", "Unsalted butter or margarine", "Fresh herbs (parsley, cilantro, basil)", "Garlic, ginger, lemon juice", "Vinegar (small amounts)" } },
                new DataSection { Title = "Other Safe Choices", Items = new List<string> { "Homemade clear soups (low sodium)", "Freshly cooked meals", "Plain crackers (low salt)" } }
            }
        };

        var column2Data = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Proteins", Items = new List<string> { "Skinless chicken or turkey", "Fish (grilled or baked)", "Lean beef or lamb (very small portions)", "Eggs (limit yolks; prefer egg whites)", "Tofu (small portions)" } },
                new DataSection { Title = "Dairy & Alternatives", Items = new List<string> { "Milk (½ cup per serving)", "Yogurt (plain, ½ cup)", "Paneer or cottage cheese (small amounts)", "Plant-based milk (check phosphorus additives)" } },
                new DataSection { Title = "Vegetables", Items = new List<string> { "Potatoes (leached before cooking)", "Sweet potatoes", "Tomatoes and tomato sauce", "Pumpkin", "Mushrooms, Okra", "Eggplant (brinjal)", "Carrots", "Beetroot" } },
                new DataSection { Title = "Fruits", Items = new List<string> { "Avocados (very small portions)", "Peaches", "Kiwi, Cherries", "Plums", "Apricots (fresh only)", "Pomegranate (limited)" } },
                new DataSection { Title = "Grains, Legumes & Fats", Items = new List<string> { "Whole wheat bread, chapati", "Brown rice, Oats", "Beans, lentils, chickpeas", "Nuts and seeds (unsalted)", "Peanut butter (small amounts)" } }
            }
        };

        var column3Data = new DataColumn
        {
            Sections = new List<DataSection> {
                new DataSection { Title = "Processed & Salty Foods", Items = new List<string> { "Bacon, sausage, hot dogs", "Deli meats and cured meats", "Fast food and fried food", "Instant noodles and packaged snacks", "Salted chips and crackers" } },
                new DataSection { Title = "Packaged & Preserved Items", Items = new List<string> { "Canned soups and vegetables", "Ready-made meals and frozen dinners", "Pickles, chutneys, relishes", "Soy sauce, ketchup, BBQ sauce", "Stock cubes and gravy mixes" } },
                new DataSection { Title = "High-Potassium Fruits", Items = new List<string> { "Bananas", "Oranges and orange juice", "Mangoes", "Papaya", "Dried fruits (dates, raisins, apricots, prunes)" } },
                new DataSection { Title = "Beverages & Sweets", Items = new List<string> { "Cola and dark sodas", "Energy drinks", "Chocolate, cocoa powder", "Sweetened packaged juices" } },
                new DataSection { Title = "Other Harmful Items", Items = new List<string> { "Salt substitutes (high potassium)", "Processed cheese and cheese spreads", "Artificial seasoning powders", "Foods with phosphate additives" } }
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
                            .Text("Renal Diet Food List".ToUpper())
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
    public float SectionTitleFontSize { get; set; } = 11f;
    public float ItemFontSize { get; set; } = 10f;
    public string PrimaryColor = "#121212";
}
