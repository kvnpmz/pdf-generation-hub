using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public interface IPdfGeneratorStrategy {
    void Generate(string fileName, float width, float height, List<DataColumn> columns, ReportTheme theme);
}

public class LayoutStrategy : IPdfGeneratorStrategy {
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
    const float ColumnSpacing = 4f;
    const float BulletPaddingTop = 1f;
    const float BulletSize = 9f;
    const int MaxTitleChars = 40;
    const int MaxItemChars = 37;
    private float H1Height;
    private float H2Height;
    private float ItemHeight;
    private float H2TotalHeight;
    const string FileTitle = "First Apartment Checklist";

    public void Generate(string fileName, float width, float height, List<DataColumn> columns, ReportTheme theme) {

        H1Height = ScaleFromLetter(36f, height);
        H2Height = ScaleFromLetter(19f, height);
        ItemHeight = ScaleFromLetter(16.0f, height);
        H2TotalHeight = H2Height + H2PaddingBottom;

        // float internalOffset = (ColumnPadding * 2) + (ColumnBorder * 2);
        float availableHeight = height - (VerticalMargin * 2) - H1Height - H1Margin - 0f;
// float availableHeight = height - (VerticalMargin * 2) - H1Height - H1Margin - HeaderTable - internalOffset - 2f;

        Document.Create(container => {
            container.Page(page => {
                page.DefaultTextStyle(x => x.FontFamily(theme.FontFamily));
                page.Size(width, height);
                //page.PageColor("#E0E0E0");
                page.MarginHorizontal(HorizontalMargin);
                page.MarginVertical(VerticalMargin);
                page.Content()
                //.Background("#FFFFFF")
                .Column(rootCol => {
                    rootCol.Item().Height(H1Height).Layers(layers => {
                        layers.PrimaryLayer().AlignTop().AlignCenter().Text(FileTitle.ToUpper())
                            .Style(TextStyle.Default.FontSize(theme.TitleFontSize).Weight(FontWeight.ExtraBold).LetterSpacing(0.045f).FontColor("#A006F8"));
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

                    rootCol.Item().Row(row => {
                        row.Spacing(ColumnSpacing);
			var colors = new[]
			{
			    "#E8F5E9", // Eat
			    "#FFFDE7", // Limit
			    "#FFEBEE", // Avoid
			};
			
			for (int i = 0; i < columns.Count; i++)
			{
			    row.RelativeItem()
				.Element(c => ColumnBox(c, colors[i % colors.Length], theme))
				.Element(c => BuildJustifiedColumn(c, availableHeight, columns[i], theme));
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

    private IContainer BlockStyle(IContainer c, string bg, string border, ReportTheme t) => 
        c.Height(HeaderTable).CornerRadius(10f).Background(bg).Border(1.3f).BorderColor(border).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.FontSize(t.HeaderTableFontSize).Bold());

    private void BuildJustifiedColumn(IContainer container, float totalHeight, DataColumn data, ReportTheme theme) {
        int totalSections = data.Sections.Count;
        int actualLineCount = 0;
        int actualTitleLineCount = 0;

        foreach (var s in data.Sections) {
            actualTitleLineCount += SplitText(s.Title, MaxTitleChars).Count;
            foreach (var item in s.Items) {
                actualLineCount += SplitText(item, MaxItemChars).Count;
            }
        }

        float usedSpace = (actualTitleLineCount * H2Height) + (totalSections * H2PaddingBottom) + (actualLineCount * ItemHeight);
        float spacerHeight = totalSections > 1 ? Math.Max(0, (totalHeight - usedSpace) / (totalSections - 1)) : 0;

        container.Column(col => {
            for (int i = 0; i < totalSections; i++) {
                var section = data.Sections[i];
                col.Item().Column(inner => {
                    var titleLines = SplitText(section.Title, MaxTitleChars);
                    inner.Item().Column(titleCol => {
                        for (int t = 0; t < titleLines.Count; t++) {
                            bool isLast = (t == titleLines.Count - 1);
                            var lineText = titleLines[t];
                            var lineContainer = titleCol.Item().Height(isLast ? H2TotalHeight : H2Height);

//                          if (isLast) {
//                              lineContainer.Layers(l => {
//                                  l.PrimaryLayer().AlignTop().Text(lineText).FontSize(theme.SectionTitleFontSize).Bold().FontColor(theme.PrimaryColor);
//                                  l.Layer().AlignBottom().PaddingBottom(H2PaddingBottom).LineHorizontal(1f).LineColor(theme.PrimaryColor);
//                              });
//                          } else {
//                              lineContainer.Text(lineText).FontSize(theme.SectionTitleFontSize).Bold().FontColor(theme.PrimaryColor);
//                          }
                            if (isLast) {
                                // 1. Shrink-wrap the container to the exact content width
                                lineContainer.Inlined(inlined => {
                                    inlined.Item().Column(titleTextCol => {
                                        // 2. Render the text naturally
                                        titleTextCol.Item().Text(lineText)
                                            .FontSize(theme.SectionTitleFontSize)
                                            .Bold()
                                            .FontColor(theme.PrimaryColor);
                                        
                                        // 3. Render the line. It is now forced to match the text width.
                                        titleTextCol.Item()
                                            .PaddingTop(H2PaddingBottom) // Replaces the bottom padding with clean top spacing
                                            .LineHorizontal(1f)
                                            .LineColor(theme.PrimaryColor);
                                    });
                                });
                            }
                        }
                    });

                    foreach (var item in section.Items) {
                        var lines = SplitText(item, MaxItemChars);
                        for (int j = 0; j < lines.Count; j++) {
                            inner.Item().Height(ItemHeight).Layers(l => {
                                if (j == 0) l.Layer().AlignLeft()
                                .PaddingTop(BulletPaddingTop)
                                .Width(BulletSize)
                                .Height(BulletSize)
                                .Border(1f)
                                .CornerRadius(1f)
                                //.CornerRadius(BulletSize / 2f)
                                .BorderColor(theme.PrimaryColor);
                                //.Background(theme.PrimaryColor)
                                l.PrimaryLayer().PaddingLeft(10f).Text(lines[j]).Bold().FontSize(theme.ItemFontSize);
                            });
                        }
                    }
                });
                if (i < totalSections - 1) col.Item().Height(spacerHeight);
            }
        });
    }

    private List<string> SplitText(string text, int max) {
        var res = new List<string>();
        if (string.IsNullOrEmpty(text)) return res;
        string remainingText = text;
        while (remainingText.Length > max) {
            int lastSpace = remainingText.LastIndexOf(' ', max);
            int breakIndex = (lastSpace == -1) ? max : lastSpace;
            res.Add(remainingText.Substring(0, breakIndex).Trim());
            remainingText = remainingText.Substring(breakIndex).Trim();
        }
        if (!string.IsNullOrWhiteSpace(remainingText)) res.Add(remainingText);
        return res;
    }
}
