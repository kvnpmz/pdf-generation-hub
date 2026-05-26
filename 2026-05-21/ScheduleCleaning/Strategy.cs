using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public enum PageSize
{
    Letter,
    A4
}

public interface IPdfGeneratorStrategy
{
    void Generate(
        string fileName,
        PageSize pageSize,
        List<DataColumn> columns,
        ReportTheme theme,
        HashSet<int>? overrideColumns = null
    );
}

public class LayoutStrategy : IPdfGeneratorStrategy
{
    const float VerticalMargin = 36f;
    const float HorizontalMargin = 50.4f;
    const float H1Margin = 0f;
    const float HeaderTable = 24f;
    const float H2PaddingBottom = 2f;
    // const float H1Height = 36f;
    // const float H2TotalHeight = H2Height + H2PaddingBottom;
    // const float H2Height = 19f;
    // const float ItemHeight = 14.5f;
    // const float ColumnPadding = 6f;
    // const float ColumnBorder = 1.3f;
    const float SectionBorder = 1.3f;
    const float SectionPadding = 0f;
    const float ColumnSpacing = 4f;
    const float BulletPaddingTop = 1f;
    const float BulletSize = 9f;
    const int MaxTitleChars = 40;
    const int MaxItemChars = 99;
    private float H1Height;
    private float H2Height;
    private float ItemHeight;
    private float H2TotalHeight;
    const string FileTitle = "New Apartment Essentials";
    const string TitleColor = "#FF1493";

    private (float width, float height) GetPageDimensions(PageSize pageSize)
    {
        return pageSize switch
        {
            PageSize.A4 => (595f, 842f),
            PageSize.Letter => (612f, 792f),
            _ => (612f, 792f)
        };
    }

    public void Generate(
        string fileName,
        PageSize pageSize,
        List<DataColumn> columns,
        ReportTheme theme,
        HashSet<int>? overrideColumns = null)
    {
        GenerateCore(fileName, pageSize, columns, theme, overrideColumns);
    }

    private void GenerateCore(string fileName, PageSize pageSize, List<DataColumn> columns, ReportTheme theme, HashSet<int>? overrideColumns)
    {
        var (width, height) = GetPageDimensions(pageSize);

        bool isA4 = pageSize == PageSize.A4;

        overrideColumns ??= new HashSet<int>();

        H1Height = ScaleFromLetter(36f, height);
        H2Height = ScaleFromLetter(15f, height);
        ItemHeight = ScaleFromLetter(10.5f, height);
        H2TotalHeight = H2Height + H2PaddingBottom;

        //float internalOffset = (ColumnPadding * 2) + (ColumnBorder * 2);
        //float availableHeight = height - (VerticalMargin * 2) - H1Height - H1Margin - 0f;
        //float availableHeight = height - (VerticalMargin * 2) - H1Height - H1Margin - HeaderTable - internalOffset - 2f;
        int sectionsCount = 6;
        float totalSectionsDecorations = sectionsCount * ((SectionPadding * 2) + (SectionBorder * 2));
        float availableHeight = height - (VerticalMargin * 2) - H1Height - H1Margin - totalSectionsDecorations - 0f;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.DefaultTextStyle(x => x.FontFamily(theme.FontFamily));
                page.Size(width, height);
                //page.PageColor("#E0E0E0");
                page.MarginHorizontal(HorizontalMargin);
                page.MarginVertical(VerticalMargin);
                page.Content()
                //.Background("#FFFFFF")
                .Column(rootCol =>
                {
                    rootCol.Item().Height(H1Height).Layers(layers =>
                    {
                        layers.PrimaryLayer().AlignTop().AlignCenter().Text(FileTitle.ToUpper())
                            .Style(TextStyle.Default.FontSize(theme.TitleFontSize).Weight(FontWeight.ExtraBold).LetterSpacing(0.046f).FontColor(TitleColor));
                        // layers.Layer().AlignBottom().PaddingBottom(3f).LineHorizontal(1.5f).LineColor(theme.PrimaryColor);
                    });

                    rootCol.Item().Height(H1Margin);

                    //                     rootCol.Item().PaddingVertical(H1Margin/2).Table(table => {
                    // 			table.ColumnsDefinition(c =>
                    // 			{
                    // 			    for (int i = 0; i < columns.Count; i++)
                    // 				c.RelativeColumn();
                    // 			});
                    // 
                    //                         table.Cell().PaddingRight(ColumnSpacing/2).Element(c => BlockStyle(c, "#b3deb6", "#2A7F2A", theme)).Text("EAT").FontColor("#2A7F2A");
                    //                         table.Cell().PaddingHorizontal(ColumnSpacing/2).Element(c => BlockStyle(c, "#fff79b", "#795901", theme)).Text("LIMIT").FontColor("#795901");
                    //                         table.Cell().PaddingLeft(ColumnSpacing/2).Element(c => BlockStyle(c, "#ff9fad", "#B22222", theme)).Text("AVOID").FontColor("#B22222");
                    //                     });

                    rootCol.Item().Row(row =>
                    {
                        row.Spacing(ColumnSpacing);
                        var colors = new[]
                        {
                "#E8F5E9", // Eat
			    "#FFFDE7", // Limit
			    "#FFEBEE", // Avoid
			};

                        for (int i = 0; i < columns.Count; i++)
                        {
                            float weight = (i == 0) ? 60f : 40f;

                            row.RelativeItem(weight)
                            .Element(c => ColumnBox(c, colors[i % colors.Length], theme))
                            .Element(c => BuildJustifiedColumn(c, availableHeight, i, columns[i], theme, isA4, overrideColumns));
                        }
                    });
                });
            });
        }).GeneratePdf($"{fileName}.pdf");
    }
    private float ScaleFromLetter(float targetLetterValue, float currentHeight)
    {
        return (targetLetterValue / 792f) * currentHeight;
    }

    private IContainer ColumnBox(IContainer c, string bg, ReportTheme t, string? border = null) => c;
    //      .CornerRadius(10f)
    //      .Border(ColumnBorder)
    //      .BorderColor(border ?? t.PrimaryColor)
    //      .Background(bg);
    //      .Padding(ColumnPadding);

    //  private IContainer BlockStyle(IContainer c, string bg, string border, ReportTheme t) => 
    //      c.Height(HeaderTable).CornerRadius(10f).Background(bg).Border(1.3f).BorderColor(border).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.FontSize(t.HeaderTableFontSize).Bold());

    private IContainer SectionBox(IContainer container, ReportTheme theme)
    {
        return container
            .Border(SectionBorder)
            .BorderColor(theme.PrimaryColor)
            .Padding(SectionPadding);
    }

    private IContainer BuildJustifiedColumn(IContainer container, float totalHeight, int colIndex, DataColumn data, ReportTheme theme, bool isA4, HashSet<int> overrideColumns)
    {
        bool shouldOverride = overrideColumns.Contains(colIndex);

        float currentItemHeight =
            (isA4 && shouldOverride)
                ? ScaleFromLetter(18.72f, totalHeight)
                : ItemHeight;

        int totalSections = data.Sections.Count;
        int actualLineCount = 0;
        int actualTitleLineCount = 0;

        foreach (var s in data.Sections)
        {
            actualTitleLineCount += SplitText(s.Title, MaxTitleChars).Count;
            foreach (var item in s.Items)
            {
                actualLineCount += SplitText(item, MaxItemChars).Count;
            }
        }

        // Space calculation remains similar, but titles and items now share horizontal planes
        float usedSpace = Math.Max(actualTitleLineCount * H2Height, actualLineCount * currentItemHeight);
        float spacerHeight = totalSections > 1 ? Math.Max(0, (totalHeight - usedSpace) / (totalSections - 1)) : 0;

        container.Column(col =>
        {
            for (int i = 0; i < totalSections; i++)
            {
                var section = data.Sections[i];
                
                // Outer Section Layout
                col.Item().Element(c => SectionBox(c, theme)).Row(row =>
                {
                    row.Spacing(8f); // Space between the Title Box and the Items List

                    // 1. LEFT SIDE: Title with its own distinct bounding box
                    row.ConstantItem(80f) // Fixed width for the title column box
                       .Background("#F5F5F5") // Optional: subtle background for the title box
                       .BorderRight(1f)
                       .BorderColor(theme.PrimaryColor)
                       .Padding(4f)
                       .AlignMiddle() // Centers title text vertically within its box
                       .AlignCenter() // Centers title text horizontally
                       .Column(titleCol =>
                       {
                           var titleLines = SplitText(section.Title, MaxTitleChars);
                           foreach (var lineText in titleLines)
                           {
                               titleCol.Item().Text(lineText)
                                   .FontSize(theme.SectionTitleFontSize)
                                   .Bold()
                                   .FontColor(theme.PrimaryColor);
                           }
                       });

                    // 2. RIGHT SIDE: The Checkbox Items
                    row.RelativeItem()
                       .Column(inner =>
                       {
                           foreach (var item in section.Items)
                           {
                               var lines = SplitText(item, MaxItemChars);
                               for (int j = 0; j < lines.Count; j++)
                               {
                                   inner.Item().Height(currentItemHeight).Layers(l =>
                                   {
                                       // Render check box on the first line of the item
                                       if (j == 0) l.Layer().AlignLeft()
                                           .PaddingTop(BulletPaddingTop)
                                           .Width(BulletSize)
                                           .Height(BulletSize)
                                           .Border(1f)
                                           .CornerRadius(1f)
                                           .BorderColor(theme.PrimaryColor);

                                       l.PrimaryLayer().PaddingLeft(14f).Text(lines[j]).Bold().FontSize(theme.ItemFontSize);
                                   });
                               }
                           }
                       });
                });

                if (i < totalSections - 1) 
                    col.Item().Height(spacerHeight);
            }
        });

        return container;
    }

    private List<string> SplitText(string text, int max)
    {
        var res = new List<string>();
        if (string.IsNullOrEmpty(text)) return res;
        string remainingText = text;
        while (remainingText.Length > max)
        {
            int lastSpace = remainingText.LastIndexOf(' ', max);
            int breakIndex = (lastSpace == -1) ? max : lastSpace;
            res.Add(remainingText.Substring(0, breakIndex).Trim());
            remainingText = remainingText.Substring(breakIndex).Trim();
        }
        if (!string.IsNullOrWhiteSpace(remainingText)) res.Add(remainingText);
        return res;
    }
}
