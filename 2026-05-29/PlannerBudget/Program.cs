using System.IO;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

QuestPDF.Settings.License = LicenseType.Community;

FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Regular.ttf"));
FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Bold.ttf"));
FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Black.ttf"));

new BudgetDocument().GeneratePDF(PageSizes.A4, "a4");
new BudgetDocument().GeneratePDF(PageSizes.Letter, "letter");

public record Section(string Title, int Rows);

public static class BudgetLayout
{
    public static readonly List<Section> LeftColumn = new()
    {
        new("Income", 6),
        new("Bills", 15),
        new("Savings", 6)
    };

    public static readonly List<Section> RightColumn = new()
    {
        new("Expenses", 15),
        new("Debt", 6),
        new("Summary", 6)
    };
}



public class BudgetDocument
{
    public void GeneratePDF(PageSize pageSize, string fileName)
    {
        float verticalMargin = 36f;
        float horizontalMargin = 50.4f;

        float headerHeight = 37f;
        float sectionHeaderHeight = 25f;

        int col1Rows = 0;
        foreach (var s in BudgetLayout.LeftColumn) col1Rows += s.Rows;

        int col2Rows = 0;
        foreach (var s in BudgetLayout.RightColumn) col2Rows += s.Rows;

        int gapsCol1 = BudgetLayout.LeftColumn.Count > 0 ? BudgetLayout.LeftColumn.Count - 1 : 0;
        int gapsCol2 = BudgetLayout.RightColumn.Count > 0 ? BudgetLayout.RightColumn.Count - 1 : 0;

        float subHeaderHeight = 25f;
        float gapSize = 8f;

        float usableHeight = pageSize.Height - (verticalMargin * 2);
        float contentAreaHeight = usableHeight - headerHeight - subHeaderHeight;

        float rowHeightCol1 = (contentAreaHeight - (BudgetLayout.LeftColumn.Count * sectionHeaderHeight) - (gapsCol1 * gapSize)) / col1Rows;
        float rowHeightCol2 = (contentAreaHeight - (BudgetLayout.RightColumn.Count * sectionHeaderHeight) - (gapsCol2 * gapSize)) / col2Rows;

        Console.WriteLine($"--- {fileName} ---");
        Console.WriteLine($"Row Height Col 1: {rowHeightCol1:F2}");
        Console.WriteLine($"Row Height Col 2: {rowHeightCol2:F2}");

        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(pageSize);
                //page.PageColor("#E0E0E0");
                page.MarginHorizontal(horizontalMargin);
                page.MarginVertical(verticalMargin);
                page.DefaultTextStyle(x => x.FontFamily("Inter").FontSize(12f).Bold().LetterSpacing(0.1f));

                page.Header().Background("#FFFFFF")
                    .Height(headerHeight)
                    .Text("Budget Planner".ToUpper())
                    .FontSize(30f)
                    .FontColor("#19437D")
                    .Black()
                    .AlignCenter();

                page.Content().Background("#FFFFFF")
                    .Column(c =>
                    {
                        c.Item()
                            .Height(subHeaderHeight)
                            .Row(row =>
                            {
                                row.RelativeItem().AlignTop().Row(r =>
                                {
                                    r.AutoItem().Text("Month: ".ToUpper());
                                    r.RelativeItem().PaddingLeft(5).BorderBottom(1).BorderColor("#000000").Text("");
                                });

                                row.ConstantItem(20);

                                row.RelativeItem().AlignTop().Row(r =>
                                {
                                    r.AutoItem().Text("Year: ".ToUpper());
                                    r.RelativeItem().PaddingLeft(5).BorderBottom(1).BorderColor("#000000").Text("");
                                });
                            });

                        c.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c1 =>
                            {
                                for (int i = 0; i < BudgetLayout.LeftColumn.Count; i++)
                                {
                                    var section = BudgetLayout.LeftColumn[i];
                                    bool isLast = (i == BudgetLayout.LeftColumn.Count - 1);

                                    c1.Item().PaddingBottom(isLast ? 0 : gapSize)
                                        .Component(new TableComponent(section.Title, section.Rows, rowHeightCol1, sectionHeaderHeight));
                                }
                            });

                            row.ConstantItem(20);

                            row.RelativeItem().Column(c2 =>
                            {
                                string[] baseLabels = { "Income", "Bills", "Savings", "Expenses", "Debt", "" };
                                string[] summaryLabels = new string[baseLabels.Length];
                                for (int i = 0; i < baseLabels.Length; i++)
                                {
                                    summaryLabels[i] = baseLabels[i].ToUpper();
                                }

                                for (int i = 0; i < BudgetLayout.RightColumn.Count; i++)
                                {
                                    var section = BudgetLayout.RightColumn[i];
                                    bool isLast = i == BudgetLayout.RightColumn.Count - 1;

                                    c2.Item().PaddingBottom(isLast ? 0 : gapSize)
                                        .Component(section.Title == "Summary"
                                            ? new TableComponent("Summary", summaryLabels, rowHeightCol2, sectionHeaderHeight)
                                            : new TableComponent(section.Title, section.Rows, rowHeightCol2, sectionHeaderHeight));
                                }
                            });
                        });
                    });
            });
        }).GeneratePdf($"budget_planner_{fileName}.pdf");
    }
}



public class TableComponent : IComponent
{
    private readonly string _title;
    private readonly string[] _rows;
    private readonly float _rowH;
    private readonly float _headerH;

    public TableComponent(string title, int count, float rowH, float headerH)
    {
        _title = title;
        _rowH = rowH;
        _headerH = headerH;

        _rows = new string[count];
        for (int i = 0; i < count; i++) 
        {
            _rows[i] = "";
        }
    }

    public TableComponent(string title, string[] rowLabels, float rowH, float headerH)
    {
        _title = title;
        _rowH = rowH;
        _headerH = headerH;
        _rows = rowLabels;
    }

    public void Compose(IContainer c) =>
        c.Column(col =>
        {
            col.Item().Height(_headerH).Row(r =>
            {
                r.ConstantItem(160f).Border(1f).AlignMiddle().Text(_title.ToUpper()).AlignCenter();
                r.RelativeItem().BorderTop(1f).BorderRight(1f).BorderBottom(1f).AlignMiddle().Text("Amount".ToUpper()).AlignCenter();
            });

            for (int i = 0; i < _rows.Length; i++)
            {
                bool isLast = (i == _rows.Length - 1);
                var label = _rows[i];

                col.Item().Height(_rowH).Row(r =>
                {
                    r.ConstantItem(160f)
                        .Border(1)
                        .BorderTop(0)
                        .BorderBottom(isLast ? 0 : 1)
                        .BorderLeft(isLast ? 0 : 1)
                        .AlignMiddle()
                        .AlignCenter()
                        .Text(text =>
                            {
                                text.DefaultTextStyle(x => x.NormalWeight());
                                text.Span(label);
                            });

                    r.RelativeItem()
                        .BorderTop(0)
                        .BorderRight(1)
                        .BorderLeft(0)
                        .BorderBottom(1)
                        .Text("");
                });
            }
        });
}

