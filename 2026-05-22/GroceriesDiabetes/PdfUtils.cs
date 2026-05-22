using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public readonly record struct LayoutMetrics(
    float H1Height,
    float H2Height,
    float ItemHeight,
    float H2TotalHeight
);

public static class TextProcessingUtils
{
    public static List<string> SplitText(string text, int max)
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

public static class PdfUtils
{
    private const float BulletPaddingTop = 1f;
    private const float BulletSize = 5f;
    private const float H2PaddingBottom = 2f;

    public static IContainer ColumnBox(IContainer c, string bg, ReportTheme t, string? border = null) => c
          //.Background(bg)
          ;
    //      .CornerRadius(10f)
    //      .Border(ColumnBorder)
    //      .BorderColor(border ?? t.PrimaryColor)
    //      .Padding(ColumnPadding);

    //  private IContainer BlockStyle(IContainer c, string bg, string border, ReportTheme t) => 
    //      c.Height(HeaderTable).CornerRadius(10f).Background(bg).Border(1.3f).BorderColor(border).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.FontSize(t.HeaderTableFontSize).Bold());

    public static void RenderSectionTitle(this ColumnDescriptor inner, string title, int currentMaxTitleChars, LayoutMetrics metrics, ReportTheme theme, int sectionIndex)
    {
        var titleLines = TextProcessingUtils.SplitText(title, currentMaxTitleChars);
        inner.Item().Column(titleCol =>
        {
            for (int t = 0; t < titleLines.Count; t++)
            {
                bool isLast = (t == titleLines.Count - 1);
                var lineText = titleLines[t].ToUpper();

                var titleBgColors = new[]
                {
                    "#d0c4df", 
                    "#b8daef",
                    "#fcd7b8"
                };

                string currentTitleBgColor = titleBgColors[sectionIndex % titleBgColors.Length];

                var lineContainer = titleCol.Item().Height(isLast ? metrics.H2TotalHeight : metrics.H2Height);

                //string currentBgColor = (t == 0) ? "#E0AFFF" : "#E0E0E0";

                if (isLast)
                {
                    lineContainer.PaddingBottom(H2PaddingBottom).Layers(layers =>
                    {
                        layers.PrimaryLayer()
                            //.Background(currentBgColor)
                            .Background(currentTitleBgColor)
                            .AlignMiddle()
                            .AlignCenter()
                            .Text(lineText)
                            .FontSize(theme.SectionTitleFontSize)
                            .Bold()
                            .FontColor(theme.PrimaryColor);

//                        layers.Layer()
//                            .AlignBottom()
//                            .LineHorizontal(1f)
//                            .LineColor(theme.PrimaryColor);
                    });
                }
                else
                {
                    lineContainer
                 //       .Background(currentBgColor)
                        .Background(currentTitleBgColor)
                        .AlignMiddle()
                        .AlignCenter()
                        .Text(lineText)
                        .FontSize(theme.SectionTitleFontSize)
                        .Bold()
                        .FontColor(theme.PrimaryColor);
                }

                //                            if (isLast) {
                //                                // 1. Shrink-wrap the container to the exact content width
                //                                lineContainer.Inlined(inlined => {
                //                                    inlined.Item().Column(titleTextCol => {
                //                                        // 2. Render the text naturally
                //                                        titleTextCol.Item().Text(lineText)
                //                                            .FontSize(theme.SectionTitleFontSize)
                //                                            .Bold()
                //                                            .FontColor(theme.PrimaryColor);
                //                                        
                //                                        // 3. Render the line. It is now forced to match the text width.
                //                                        titleTextCol.Item()
                //                                            .PaddingTop(H2PaddingBottom) // Replaces the bottom padding with clean top spacing
                //                                            .LineHorizontal(1f)
                //                                            .LineColor(theme.PrimaryColor);
                //                                    });
                //                                });
                //                            }
            }
        });
    }

    public static void RenderSectionItems(this ColumnDescriptor inner, List<string> items, int currentMaxItemChars, float currentItemHeight, ReportTheme theme)
    {
        foreach (var item in items)
        {
            var lines = TextProcessingUtils.SplitText(item, currentMaxItemChars);
            for (int j = 0; j < lines.Count; j++)
            {
                var currentLineText = lines[j];
                bool isFirstLineOfItem = (j == 0);

                inner.Item().Height(currentItemHeight).Layers(l =>
                {
                    if (isFirstLineOfItem) l.Layer().AlignLeft()
                        .PaddingTop(BulletPaddingTop)
                        .Width(BulletSize)
                        .Height(BulletSize)
                        .Border(0.7f)
                        //.CornerRadius(1f)
                        //.CornerRadius(BulletSize / 2f)
                        .BorderColor(theme.PrimaryColor);
                    //.Background(theme.PrimaryColor)
                    l
                    .PrimaryLayer()
                    .PaddingLeft(8f)
                    .Text(currentLineText.ToUpper())
                    //.Bold()
                    .FontSize(theme.ItemFontSize);
                });
            }
        }
    }
}
