using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
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
    //const float H2PaddingBottom = 2f;
    //const float H1Height = 36f;
    //const float H2TotalHeight = H2Height + H2PaddingBottom;
    //const float H2Height = 19f;
    //const float ItemHeight = 14.5f;
    //const float ColumnPadding = 6f;
    //const float ColumnBorder = 1.3f;
    //private float H2TotalHeight;
    const float ColumnSpacing = 4f;
    const int MaxTitleChars = 20;
    const int MaxItemChars = 14;
    const string FileTitle = "Diabetes Grocery List";
    const string TitleColor = "#4275a7";
    private const float BaseH1Height = 36f;
    private const float BaseH2Height = 10.0f;
    private const float BaseItemHeight = 13.5f;
    private const float H2PaddingBottom = 2f;

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

        // 4. Calculate metrics locally in a single place
        var metrics = new LayoutMetrics(
          H1Height: ScaleFromLetter(BaseH1Height, height),
          H2Height: ScaleFromLetter(BaseH2Height, height),
          ItemHeight: ScaleFromLetter(BaseItemHeight, height),
          H2TotalHeight: ScaleFromLetter(BaseH2Height, height) + H2PaddingBottom
        );

        float availableHeight = height - (VerticalMargin * 2) - metrics.H1Height - H1Margin - 0f;
        // float internalOffset = (ColumnPadding * 2) + (ColumnBorder * 2);
        // float availableHeight = height - (VerticalMargin * 2) - H1Height - H1Margin - HeaderTable - internalOffset - 2f;

        var globalCounter = new SectionCounter();

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
                        .Background("#FFFFFF")
                        .Column(rootCol =>
                        {
                            rootCol.Item().Height(metrics.H1Height).Layers(layers =>
                            {
                                layers.PrimaryLayer().AlignTop().AlignCenter().Text(FileTitle.ToUpper())
                                  .Style(TextStyle.Default.FontSize(theme.TitleFontSize).Weight(FontWeight.ExtraBold).LetterSpacing(0.046f).FontColor(TitleColor));
                                // layers.Layer().AlignBottom().PaddingBottom(3f).LineHorizontal(1.5f).LineColor(theme.PrimaryColor);
                            });

                            rootCol.Item().Height(H1Margin);

                            //                     rootCol.Item().PaddingVertical(H1Margin/2).Table(table => {
                            //             table.ColumnsDefinition(c =>
                            //             {
                            //                 for (int i = 0; i < columns.Count; i++)
                            //                 c.RelativeColumn();
                            //             });
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
                                    int colIndex = i;
                                    row.RelativeItem()
                                        .Element(c => PdfUtils.ColumnBox(c, colors[colIndex % colors.Length], theme))
                                        .Element(c => BuildJustifiedColumn(c, availableHeight, colIndex, columns[colIndex], theme, isA4, overrideColumns, metrics, globalCounter));
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

    private void BuildJustifiedColumn(IContainer container, float totalHeight, int colIndex, DataColumn data, ReportTheme theme, bool isA4, HashSet<int> overrideColumns, LayoutMetrics metrics, SectionCounter counter) 
    {
        bool shouldOverride = overrideColumns.Contains(colIndex);

        int currentMaxTitleChars = colIndex switch
        {
           // 3 => 15,
            4 => 15,
            _ => MaxTitleChars
        };
        int currentMaxItemChars = MaxItemChars;

        float currentItemHeight =
          (isA4 && shouldOverride)
            ? ScaleFromLetter(18.72f, totalHeight)
            : metrics.ItemHeight;

        int totalSections = data.Sections.Count;
        int actualLineCount = 0;
        int actualTitleLineCount = 0;

        foreach (var s in data.Sections)
        {
            actualTitleLineCount += TextProcessingUtils.SplitText(s.Title, currentMaxTitleChars).Count;
            foreach (var item in s.Items)
            {
                actualLineCount += TextProcessingUtils.SplitText(item, currentMaxItemChars).Count;
            }
        }

        float usedSpace = (actualTitleLineCount * metrics.H2Height) + (totalSections * H2PaddingBottom) + (actualLineCount * currentItemHeight);
        float spacerHeight = totalSections > 1 ? Math.Max(0, (totalHeight - usedSpace) / (totalSections - 1)) : 0;

        container.Column(col =>
        {
            for (int i = 0; i < totalSections; i++)
            {
                var section = data.Sections[i];

                int currentSectionIndex = counter.Value;
                counter.Value++;

                col.Item().Column(inner =>
                {
                    inner.RenderSectionTitle(section.Title, currentMaxTitleChars, metrics, theme, currentSectionIndex);
                    inner.RenderSectionItems(section.Items, currentMaxItemChars, currentItemHeight, theme);
                });
                if (i < totalSections - 1) col.Item().Height(spacerHeight);
            }
        });
    }
    public class SectionCounter
    {
        public int Value { get; set; } = 0;
    }
}
