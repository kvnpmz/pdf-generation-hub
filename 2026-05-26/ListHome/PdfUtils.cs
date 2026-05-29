using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public readonly record struct LayoutMetrics(
    float HeaderHeight,
    float SubheaderHeight,
    float TitleHeight,
    float SubtitleHeight,
    float ItemHeight
    //float HeaderRulerHeight
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
    public static IContainer ColumnBox(IContainer c, string bg, ReportTheme t, string? border = null) => c;


    public static void RenderTitle(this ColumnDescriptor inner, string title, int currentMaxTitleChars, LayoutMetrics metrics, ReportTheme theme, int sectionIndex)
    {
        var titleLines = TextProcessingUtils.SplitText(title, currentMaxTitleChars);
        inner.Item().Column(titleCol =>
        {
            for (int t = 0; t < titleLines.Count; t++)
            {
                bool isLast = (t == titleLines.Count - 1);
                var lineText = titleLines[t].ToUpper();

//                var titleBgColors = new[]
//                {
//                    "#d0c4df",
//                    "#b8daef",
//                    "#fcd7b8"
//                };
//
//                string currentTitleBgColor = titleBgColors[sectionIndex % titleBgColors.Length];

                //string titleBgColor = "#ebafb0";

                titleCol.Item()
                    .Height(metrics.TitleHeight)
                    .Layers(layers =>
                    {
                        layers.PrimaryLayer()
                        //.Background(titleBgColor)
                        .AlignCenter()
                        .Text(lineText)
                        .FontSize(theme.TitleFontSize)
                        .Bold()
                        .FontColor(theme.PrimaryColor);

                    if (isLast)
                    {
                        layers.Layer()
                            .BorderBottom(1.5f)
                            .BorderColor(theme.PrimaryColor);
                    }
                });
            }
        });
    }

    public static void RenderSubtitle(this ColumnDescriptor inner, string subtitle, int currentMaxSubtitleChars, LayoutMetrics metrics, ReportTheme theme, int sectionIndex)
    {
        var subtitleLines = TextProcessingUtils.SplitText(subtitle, currentMaxSubtitleChars);
        inner.Item().Column(subtitleCol =>
        {
            for (int t = 0; t < subtitleLines.Count; t++)
            {
                bool isLast = (t == subtitleLines.Count - 1);
                var lineText = subtitleLines[t].ToUpper();

                subtitleCol.Item()
                    .Height(metrics.SubtitleHeight)
                    .Layers(layers =>
                    {
                        layers.PrimaryLayer()
                            .AlignCenter()
                            .Text(lineText)
                            .FontSize(theme.SubtitleFontSize)
                            .Bold()
                            .FontColor(theme.PrimaryColor);

                        if (isLast)
                        {
                            layers.Layer()
                                .BorderBottom(0.5f)
                                .BorderColor(theme.PrimaryColor);
                        }
                    });
            }
        });
    }

    public static void RenderItems(this ColumnDescriptor inner, List<string> items, int currentMaxItemChars, float currentItemHeight, ReportTheme theme)
    {
        foreach (var item in items)
        {
            bool isBlankLineSymbol = item.Trim().StartsWith("___");

            if (isBlankLineSymbol)
            {
                inner.Item().Height(currentItemHeight).Layers(l =>
                {
                    l.Layer()
                        .AlignLeft()
                        .AlignMiddle()
                        .PaddingTop(theme.BulletPaddingTop)
                        .Width(theme.BulletSize)
                        .Height(theme.BulletSize)
                        .Border(theme.BulletThickness)
                        .BorderColor(theme.PrimaryColor);

                    l.PrimaryLayer()
                        .AlignBottom()
                        .PaddingLeft(theme.ItemPaddingLeft) 
                        .BorderBottom(1.5f)
                        .BorderColor(theme.PrimaryColor);
                    });
                continue;
            }

            var lines = TextProcessingUtils.SplitText(item, currentMaxItemChars);
            for (int j = 0; j < lines.Count; j++)
            {
                var currentLineText = lines[j];
                bool isFirstLineOfItem = (j == 0);

                inner.Item().Height(currentItemHeight).Layers(l =>
                {
                    if (isFirstLineOfItem) l.Layer()
                        .AlignLeft()
                        .AlignMiddle()
                        .PaddingTop(theme.BulletPaddingTop)
                        .Width(theme.BulletSize)
                        .Height(theme.BulletSize)
                        .Border(theme.BulletThickness)
                        .BorderColor(theme.PrimaryColor);

                    l
                    .PrimaryLayer()
                    .AlignMiddle()
                    .PaddingLeft(theme.ItemPaddingLeft)
                    .Text(currentLineText
                    //.ToUpper()
                    )
                    .FontSize(theme.ItemFontSize);
                });
            }
        }
    }
}
