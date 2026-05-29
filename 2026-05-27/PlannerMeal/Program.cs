using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

QuestPDF.Settings.License = LicenseType.Community;

FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Regular.ttf"));
FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Bold.ttf"));
FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Black.ttf"));

var letterSize = new PageSize(612f, 792f);
var a4Size = new PageSize(595f, 842f);

GenerateMealPlanner("meal_planner_a4.pdf", a4Size);
GenerateMealPlanner("meal_planner_letter.pdf", letterSize);

static void GenerateMealPlanner(string filePath, PageSize size)
{
    float verticalMargin = 36f;
    float horizontalMargin = 50.4f;
    float headerHeight = 37f;
    float subheaderHeight = 25f;
    float gapSpace = 10f;

    float totalPageHeight = size.Height;
    float availableContentHeight = totalPageHeight - (verticalMargin * 2) - headerHeight - subheaderHeight - gapSpace;

    Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(size);
            //page.PageColor("#E0E0E0");
            page.MarginVertical(verticalMargin);
            page.MarginHorizontal(horizontalMargin);
            page.DefaultTextStyle(x => x.FontFamily("Inter").FontColor(Theme.Color).LetterSpacing(0.30f));

            page.Content().Background("#FFFFFF").Column(col =>
            {
                col.Item().Height(headerHeight).AlignCenter().Text("Weekly Meal Plan".ToUpper())
                    .Style(TextStyle.Default
                        .FontSize(30f)
                        .Weight(FontWeight.Black));

                col.Item().PaddingTop(gapSpace * 0.1f).Component(new WeekOfSubheader(subheaderHeight));

                col.Item().PaddingTop(gapSpace * 0.9f).Height(availableContentHeight).Row(row =>
                {
                    row.RelativeItem(1).PaddingRight(10).Column(leftCol =>
                    {
                        var days = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                        for (int i = 0; i < days.Length; i++)
                        {
                            days[i] = days[i].ToUpper();
                        }

                        var colors = new[] { "#E1EAF2", "#C4DBC4", "#FBE3C4", "#FCC7D0", "#FAD8C5", "#E4D0E5", "#FDC7CF" };

                        float singleDayHeight = availableContentHeight / days.Length;

                        for (int i = 0; i < days.Length; i++)
                        {
                            leftCol.Item().Height(singleDayHeight).Component(new DayMealBlock(days[i], colors[i], singleDayHeight));
                        }
                    });
                    row.RelativeItem(1).PaddingLeft(10).Column(rightCol =>
                    {
                        float shoppingTitleHeight = 20f;

                        rightCol.Item().Height(shoppingTitleHeight)
                            .BorderTop(1f)
                            .AlignCenter().Text("Shopping List".ToUpper())
                            .FontSize(14f)
                            .Bold();

                        float availableTableHeight = availableContentHeight - shoppingTitleHeight;
                        float singleRowHeight = availableTableHeight / 3f;

                        rightCol.Item()
                            .Height(availableTableHeight)
                            .Column(gridCol =>
                            {
                                gridCol.Item().Height(singleRowHeight).Row(r =>
                                {
                                    r.RelativeItem(1).Component(new ShoppingCategoryBlock("Dairy".ToUpper(), "#FCC7CF", singleRowHeight));
                                    r.RelativeItem(1).Component(new ShoppingCategoryBlock("Produce".ToUpper(), "#F7E0BD", singleRowHeight));
                                });

                                gridCol.Item().Height(singleRowHeight).Row(r =>
                                {
                                    r.RelativeItem(1).Component(new ShoppingCategoryBlock("Grains".ToUpper(), "#DDEFF6", singleRowHeight));
                                    r.RelativeItem(1).Component(new ShoppingCategoryBlock("Meat".ToUpper(), "#C4DBC4", singleRowHeight));
                                });

                                gridCol.Item().Height(singleRowHeight).Row(r =>
                                {
                                    r.RelativeItem(1).Component(new ShoppingCategoryBlock("Frozen".ToUpper(), "#FAD8C5", singleRowHeight));
                                    r.RelativeItem(1).Component(new ShoppingCategoryBlock("MISC.", "#E3CFE4", singleRowHeight));
                                });
                            });
                    });
                });
            });
        });
    }).GeneratePdf(filePath);
}

class WeekOfSubheader : IComponent
{
    private readonly float _height;
    public WeekOfSubheader(float height) => _height = height;

    public void Compose(IContainer container)
    {
        container
            .Border(1f)
            .Background("#EAEDE5")
            .Height(_height)
            .AlignLeft()
            .AlignMiddle()
            .PaddingLeft(10f)
            .Text("Week Of:".ToUpper())
            .Bold()
            .FontSize(10f);
    }
}

class DayMealBlock : IComponent
{
    private readonly string _day;
    private readonly string _hexColor;
    private readonly float _totalHeight;

    public DayMealBlock(string day, string hexColor, float totalHeight)
    {
        _day = day;
        _hexColor = hexColor;
        _totalHeight = totalHeight;
    }

    public void Compose(IContainer container)
    {
        float titleHeight = 16f;
        float mealsAreaHeight = _totalHeight - titleHeight - 4f;
        float singleMealRowHeight = mealsAreaHeight / 3f;

        container.Height(_totalHeight).Border(1f).Background(_hexColor).Column(col =>
        {
            col.Item()
                .Height(titleHeight)
                .BorderBottom(1f)
                .Background(Colors.White)
                .AlignCenter()
                .AlignMiddle()
                .Text(_day)
                .FontSize(10f)
                .Bold();

            col.Item().Height(mealsAreaHeight).PaddingHorizontal(6).Column(meals =>
            {
                var mealTypes = new[] { "B", "L", "D" };

                foreach (var type in mealTypes)
                {
                    meals.Item().Height(singleMealRowHeight).Layers(layers =>
                    {
                        layers.PrimaryLayer().Height(singleMealRowHeight);

                        layers.Layer().AlignBottom().Text(".................................")
                            .FontColor(Theme.Color);

                        layers.Layer().AlignMiddle().Text(type).Bold().FontSize(12);
                    });
                }
            });
        });
    }
}

class ShoppingCategoryBlock : IComponent
{
    private readonly string _header;
    private readonly string _hexColor;
    private readonly float _blockHeight;

    public ShoppingCategoryBlock(string header, string hexColor, float blockHeight)
    {
        _header = header;
        _hexColor = hexColor;
        _blockHeight = blockHeight;
    }

    public void Compose(IContainer container)
    {
        float subTitleHeight = 16f;
        float internalInnerPadding = 4f;
        int targetLines = 9;

        float linesAreaHeight = _blockHeight - subTitleHeight;
        float innerContentHeight = linesAreaHeight - (internalInnerPadding * 2);
        float lineRowHeight = innerContentHeight / targetLines;

        container.Height(_blockHeight).Border(1f).Column(col =>
        {
            col.Item().Height(subTitleHeight).BorderBottom(2f).AlignCenter().AlignMiddle()
                .Text(_header).FontSize(10f).Bold();

            float dynamicHeight = Math.Max(0, linesAreaHeight);

            col.Item().Height(linesAreaHeight).Background(_hexColor).Padding(internalInnerPadding).Column(lines =>
            {
                for (int i = 0; i < targetLines; i++)
                {
                    lines.Item().Height(lineRowHeight).Layers(layers =>
                    {
                        layers.PrimaryLayer().Height(lineRowHeight);

                        layers.Layer().AlignBottom().Text("................")
                            .FontColor(Theme.Color);

                        float boxSize = 9f;

                        layers.Layer()
                            .AlignMiddle()
                            .PaddingBottom(4f)
                            .PaddingLeft(1.0f)
                            .Row(row =>
                            {
                                row.ConstantItem(boxSize)
                                    .Width(boxSize)
                                    .Height(boxSize)
                                    .Border(1f)
                                    .CornerRadius(boxSize / 2f)
                                    .BorderColor(Theme.Color)
                                    .Background("#FFFFFF");
                            });
                    });
                }
            });
        });
    }
}

public static class Theme
{
    public const string Color = "#121212";
}
