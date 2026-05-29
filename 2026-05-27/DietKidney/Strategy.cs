using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public enum PageSize { Letter, A4 }

public interface IPdfGeneratorStrategy
{
    void Generate(string fileName, string fileHeader, PageSize pageSize, List<DataColumn> columns, ReportTheme theme);
}

public class LayoutStrategy : IPdfGeneratorStrategy
{

    private readonly string[] _columnBackgroundColors = { "#E8F5E9", "#FFFDE7", "#FFEBEE" };

    public void Generate(string fileName, string fileHeader, PageSize pageSize, List<DataColumn> columns, ReportTheme theme)
    {
        string? subheaderText = null;

        var dimensions = PageDimensions.FromPageSize(pageSize);

        var layoutMetrics = new LayoutMetrics(
            HeaderHeight: theme.BaseHeaderHeight * dimensions.ScaleFactor,
            SubheaderHeight: theme.BaseSubheaderHeight * dimensions.ScaleFactor,
            TitleHeight: theme.BaseTitleHeight * dimensions.ScaleFactor,
            SubtitleHeight: theme.BaseSubtitleHeight * dimensions.ScaleFactor,
            ItemHeight: theme.BaseItemHeight * dimensions.ScaleFactor
            //HeaderRulerHeight: theme.BaseHeaderRulerHeight * dimensions.ScaleFactor
        );

        var headerLines = TextProcessingUtils.SplitText(fileHeader, theme.MaxHeaderChars);
        float totalHeaderHeight = headerLines.Count * layoutMetrics.HeaderHeight;

        float totalSubheaderHeight = 0;
        var subheaderLines = new List<string>();

        if (!string.IsNullOrEmpty(subheaderText))
        {
            subheaderLines = TextProcessingUtils.SplitText(subheaderText, theme.MaxSubheaderChars);
            totalSubheaderHeight = subheaderLines.Count * layoutMetrics.SubheaderHeight;
        }
 
        float subheaderTableHeight = layoutMetrics.SubheaderHeight; 

        float totalColumnPadding = 2 * theme.ColumnPadding;

        float availableHeight = dimensions.Height - (theme.VerticalMargin * 2) - totalHeaderHeight - totalSubheaderHeight - subheaderTableHeight - totalColumnPadding;

        var globalCounter = new SectionCounter();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.DefaultTextStyle(x => x.FontFamily(theme.FontFamily));
                page.Size(dimensions.Width, dimensions.Height);
                //page.PageColor("#E0E0E0");
                page.MarginHorizontal(theme.HorizontalMargin);
                page.MarginVertical(theme.VerticalMargin);

                page.Content().Background("#FFFFFF").Column(rootCol =>
                {
                    rootCol.Item().Column(headerCol =>
                    {
                        foreach (var lineText in headerLines)
                        {
                            headerCol.Item().Height(layoutMetrics.HeaderHeight).Layers(layers =>
                            {
                                layers.PrimaryLayer()
                                .AlignTop()
                                .AlignCenter()
                                .Text(lineText.ToUpper())
                                .Style(TextStyle.Default.FontSize(theme.HeaderFontSize)
                                .Weight(FontWeight.Black)
                                .LetterSpacing(theme.HeaderLetterSpacing)
                                .FontColor(theme.HeaderColor));

                                layers.Layer()
                                    .PaddingTop(35f)
                                    .BorderTop(1.5f)
                                    .BorderColor(theme.HeaderColor);
                            });
                        }
                    });

                    if (subheaderLines.Count > 0)
                    {
                        rootCol.Item().Column(subheaderCol =>
                        {
                            foreach (var lineText in subheaderLines)
                            {
                                subheaderCol.Item().Height(layoutMetrics.SubheaderHeight).Layers(layers =>
                                {
                                    layers.PrimaryLayer().AlignTop().AlignCenter().Text(lineText.ToUpper())
                                        .Style(TextStyle.Default.FontSize(theme.SubheaderFontSize).Weight(FontWeight.ExtraBold).LetterSpacing(0.046f).FontColor(theme.SubheaderColor));
                                });
                            }
                        });
                    }

                    float cellHeight = layoutMetrics.SubheaderHeight - 5f;
                    float cellCornerRadius = 5f;

                    rootCol.Item()
                        .Height(layoutMetrics.SubheaderHeight)
                        .AlignMiddle()
                        .DefaultTextStyle(x => x.FontSize(theme.SubheaderFontSize).Bold())
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columnsDef =>
                            {
                                columnsDef.RelativeColumn();
                                columnsDef.RelativeColumn();
                                columnsDef.RelativeColumn();
                            });

                            table.Cell().PaddingRight(theme.ColumnSpacing / 2)
                                .Height(cellHeight)
                                .Border(1f)
                                .BorderColor(theme.PrimaryColor)
                                .CornerRadius(cellCornerRadius )
                                .Background("#b3deb6")
                                .AlignMiddle()
                                .AlignCenter() 
                                .Text("EAT".ToUpper()).FontColor("#2A7F2A");

                            table.Cell().PaddingHorizontal(theme.ColumnSpacing / 2)
                                .Height(cellHeight)
                                .Border(1f)
                                .BorderColor(theme.PrimaryColor)
                                .CornerRadius(cellCornerRadius)
                                .Background("#fff79b")
                                .AlignMiddle()
                                .AlignCenter()
                                .Text("LIMIT".ToUpper()).FontColor("#795901");

                            table.Cell().PaddingLeft(theme.ColumnSpacing / 2)
                                .Height(cellHeight)
                                .Border(1f)
                                .BorderColor(theme.PrimaryColor)
                                .CornerRadius(cellCornerRadius )
                                .Background("#ff9fad")
                                .AlignMiddle()
                                .AlignCenter()
                                .Text("AVOID".ToUpper()).FontColor("#B22222");
                        });

                    rootCol.Item().Row(row =>
                    {
                        row.Spacing(theme.ColumnSpacing);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            int colIndex = i;
                            string bgColor = _columnBackgroundColors[colIndex % _columnBackgroundColors.Length];

                            row.RelativeItem()
                                .Element(c => PdfUtils.ColumnBox(c, bgColor, theme))
                                .Element(c => RenderColumnContent(c, availableHeight, colIndex, columns[colIndex], theme, dimensions, layoutMetrics, globalCounter));
                        }
                    });
                });
            });
        }).GeneratePdf($"{fileName}.pdf");
    }

    private void RenderColumnContent(IContainer container, float availableHeight, int colIndex, DataColumn data, ReportTheme theme, PageDimensions dimensions, LayoutMetrics metrics, SectionCounter counter)
    {
        var ColumnContext = ColumnLayoutCalculator.Calculate(availableHeight, colIndex, data, theme, dimensions, metrics);

        //Console.WriteLine($"Column [{colIndex}] ({dimensions.Size}): Calculated Item Height = {ColumnContext.ItemHeight:F2}, Spacer Height = {ColumnContext.SpacerHeight:F2}");

        container.Column(col =>
        {
            for (int i = 0; i < data.Sections.Count; i++)
            {
                var section = data.Sections[i];
                int currentSectionIndex = counter.Value++;

                col.Item().Column(inner =>
                {
                    inner.RenderTitle(section.Title, ColumnContext.MaxTitleChars, metrics, theme, currentSectionIndex);

                    foreach (var item in section.Items)
                    {
                        if (string.IsNullOrEmpty(item)) continue;

                        if (item.StartsWith("#"))
                        {
                            string cleanSubtitle = item.TrimStart('#').Trim();
                            inner.RenderSubtitle(cleanSubtitle, ColumnContext.MaxSubtitleChars, metrics, theme, currentSectionIndex);
                        }
                        else
                        {
                            var singleItemWrapper = new List<string> { item };
                            inner.RenderItems(singleItemWrapper, ColumnContext.MaxItemChars, ColumnContext.ItemHeight, theme);
                        }
                    }
                });

                if (i < data.Sections.Count - 1)
                {
                    col.Item().Height(ColumnContext.SpacerHeight);
                }
            }
        });
    }

    public class SectionCounter
    {
        public int Value { get; set; } = 0;
    }
}

#region Helper Layout Models (Separation of Concerns)

public record PageDimensions(float Width, float Height, float ScaleFactor, PageSize Size)
{

    private const float LetterHeight = 792f;
    private const float LetterWidth = 612f;
    private const float A4Height = 842f;
    private const float A4Width = 595f;

    public static PageDimensions FromPageSize(PageSize pageSize)
    {
        var (w, h) = pageSize switch
        {
            PageSize.A4 => (A4Width, A4Height),
            PageSize.Letter => (LetterWidth, LetterHeight),
            _ => (LetterWidth, LetterHeight)
        };

        return new PageDimensions(w, h, h / LetterHeight, pageSize);
    }
}

public record ColumnLayoutContext(int MaxTitleChars, int MaxSubtitleChars, int MaxItemChars, float ItemHeight, float SpacerHeight);

public static class ColumnLayoutCalculator
{
    public static ColumnLayoutContext Calculate(float totalHeight, int colIndex, DataColumn data, ReportTheme theme, PageDimensions dimensions, LayoutMetrics metrics)
    {
        int maxTitleChars = colIndex switch
        {
            _ => theme.MaxTitleChars 
        };

        int maxItemChars = colIndex switch
        {
            //4 => 14,
            _ => theme.MaxItemChars 
        };

        int maxSubtitleChars = theme.MaxSubtitleChars;

        bool shouldOverride = dimensions.Size == PageSize.A4 && new int[] { 0, 1, 2 }.Contains(colIndex);
        float itemHeight = shouldOverride ? 17.0f * (totalHeight / totalHeight) : metrics.ItemHeight;

        int actualItemLineCount = 0;
        int actualTitleLineCount = 0;
        int actualSubtitleLineCount = 0;

        foreach (var s in data.Sections)
        {
            actualTitleLineCount += TextProcessingUtils.SplitText(s.Title, maxTitleChars).Count;

            foreach (var item in s.Items)
            {
                if (string.IsNullOrEmpty(item)) continue;

                if (item.StartsWith("#"))
                {
                    string cleanSubtitle = item.TrimStart('#').Trim();
                    actualSubtitleLineCount += TextProcessingUtils.SplitText(cleanSubtitle, maxSubtitleChars).Count;
                }
                else
                {
                    actualItemLineCount += TextProcessingUtils.SplitText(item, maxItemChars).Count;
                }
            }
        }

        float usedSpace = (actualTitleLineCount * metrics.TitleHeight) + (actualSubtitleLineCount * metrics.SubtitleHeight) + (actualItemLineCount * itemHeight);

        float spacerHeight = data.Sections.Count > 1 ? Math.Max(0, (totalHeight - usedSpace) / (data.Sections.Count - 1)) : 0;

        return new ColumnLayoutContext(maxTitleChars, maxSubtitleChars, maxItemChars, itemHeight, spacerHeight);
    }
}
#endregion
