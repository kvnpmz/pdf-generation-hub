using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

QuestPDF.Settings.License = LicenseType.Community;

FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Regular.ttf"));
FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Bold.ttf"));
FontManager.RegisterFont(File.OpenRead("/usr/local/share/fonts/inter-ttf/Inter-Black.ttf"));

var letterDocument = new PaycheckBudgetDocument(DocumentSize.Letter);
letterDocument.GeneratePdf("paycheck_budget_letter.pdf");

var a4Document = new PaycheckBudgetDocument(DocumentSize.A4);
a4Document.GeneratePdf("paycheck_budget_a4.pdf");

Console.WriteLine("Documents generated successfully.");

public static class Theme
{
    public static class Fonts
    {
        public const string Family = "Inter";

        public const float HeaderSize = 30f;
        public const float TitleSize = 12f;
        public const float BodySize = 10f;
    }

    public static class Colors
    {
        public const string Gray = "#E0E0E0";
        public const string ContentBackground = "#FFFFFF";
        public const string Primary = "#121212";
    }

    public static class Borders
    {
        public const float Thin = 1f;
        public const float Thick = 1.5f;
    }
}

public static class Layout
{
    public static class Margin
    {
        public const float Vertical = 36f;
        public const float Horizontal = 50.4f;
    }

    public static class Spacing
    {
        public const float Vertical = 8f;
        public const float Horizontal = 8f;
        public const float FooterLabel = 8f;
    }

    public static class Heights
    {
        public const float MonthsHeader = 15f;
        public const float MainHeader = 37f;
        public const float IncomeSection = 80f;
    }

    public struct ColumnWidths
    {
        public const float Due = 40f;
        public const float Paid = 36f;
        public const float Default = 60f;
        public const float Summary = 90f;
    }

    public static class Weights
    {
        public const float LeftPanel = 1.1f;
        public const float RightPanel = 0.9f;
    }
}

public class PaycheckBudgetDocument : IDocument
{
    private readonly PageSize _pageSize;

    public PaycheckBudgetDocument(PageSize pageSize)
    {
        _pageSize = pageSize;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(_pageSize);

            page.MarginVertical(Layout.Margin.Vertical);
            page.MarginHorizontal(Layout.Margin.Horizontal);
            //page.PageColor(Theme.Colors.Gray);

            page.DefaultTextStyle(x => x
                .FontFamily(Theme.Fonts.Family)
                .FontSize(Theme.Fonts.BodySize)
                .FontColor(Theme.Colors.Primary)
                .Bold());

            float totalHeight = _pageSize.Height;
            float usableHeight = totalHeight - (Layout.Margin.Vertical * 2);

            float monthsRowHeight = Layout.Heights.MonthsHeader;
            float mainHeaderHeight = Layout.Heights.MainHeader;
            float incomeSectionHeight = Layout.Heights.IncomeSection;
            float gaps = Layout.Spacing.Vertical * 3;

            float remainingHeight = usableHeight - monthsRowHeight - mainHeaderHeight - incomeSectionHeight - gaps;
            float leftColumnHeight = remainingHeight;
            float rightColumnHeight = remainingHeight;

            page.Content().Background(Theme.Colors.ContentBackground).Column(column =>
            {
                column.Spacing(Layout.Spacing.Vertical);

                column.Item().Height(monthsRowHeight).Component(new MonthsHeaderComponent());
                column.Item().Height(mainHeaderHeight).Component(new MainTitleComponent());
                column.Item().Height(incomeSectionHeight).Component(new IncomeSectionComponent(incomeSectionHeight));

                column.Item().Height(remainingHeight).Row(row =>
                {
                    row.Spacing(Layout.Spacing.Horizontal);

                    row.RelativeItem(Layout.Weights.LeftPanel).Column(leftCol =>
                    {
                        leftCol.Spacing(Layout.Spacing.Vertical);

                        float leftGap = Layout.Spacing.Vertical;
                        float availableLeftHeight = leftColumnHeight - leftGap;
                        float billsHeight = availableLeftHeight * SectionRatios.Half;
                        float spendingHeight = availableLeftHeight * SectionRatios.Half;

                        var billsCols = new TrackerColumn[]
                        {
                            new() { Header = "Due", Width = Layout.ColumnWidths.Due, Padding = (2f, 2f) },
                            new() { Header = "Amount", Width = Layout.ColumnWidths.Default, Padding = (2f, 2f) },
                            new() { Header = "Paid", Width = Layout.ColumnWidths.Paid, Padding = (10f, 4f) }
                        };

                        float billsFooterWidth = Layout.ColumnWidths.Default - 2f + Layout.ColumnWidths.Paid;
                        float billsFooterPadding = Layout.ColumnWidths.Paid + 2f;

                        leftCol.Item().Height(billsHeight).Component(new TrackerComponent(
                            "Bills", 10, billsCols, billsHeight, "Total Bills Paid", billsFooterWidth, billsFooterPadding));

                        var spendingCols = new TrackerColumn[]
                        {
                            new() { Header = "Budget", Width = Layout.ColumnWidths.Default, Padding = (2f, 2f) },
                            new() { Header = "Actual", Width = Layout.ColumnWidths.Default, Padding = (2f, 2f) }
                        };

                        leftCol.Item().Height(spendingHeight).Component(new TrackerComponent(
                            "Spending", 10, spendingCols, spendingHeight, "Total Spending",
                            Layout.ColumnWidths.Default - 2f, 2f));
                    });

                    row.RelativeItem(Layout.Weights.RightPanel).Column(rightCol =>
                    {
                        rightCol.Spacing(Layout.Spacing.Vertical);

                        float rightGap = Layout.Spacing.Vertical * 2;
                        float availableRightHeight = rightColumnHeight - rightGap;
                        float savingsHeight = availableRightHeight * SectionRatios.Savings;
                        float debtsHeight = availableRightHeight * SectionRatios.Debts;
                        float summaryHeight = availableRightHeight * SectionRatios.Summary;

                        var savingsCols = new TrackerColumn[]
                        {
                            new() { Header = "Amount", Width = Layout.ColumnWidths.Default, Padding = (2f, 2f) }
                        };

                        rightCol.Item().Height(savingsHeight).Component(new TrackerComponent(
                            "Savings Goal", 6, savingsCols, savingsHeight, "Total Savings",
                            Layout.ColumnWidths.Default - 2f, 2f));

                        var debtCols = new TrackerColumn[]
                        {
                            new() { Header = "Amount", Width = Layout.ColumnWidths.Default, Padding = (2f, 2f) }
                        };

                        rightCol.Item().Height(debtsHeight).Component(new TrackerComponent(
                            "Extra Debts", 6, debtCols, debtsHeight, "Total Debt Paid",
                            Layout.ColumnWidths.Default - 2f, 2f));

                        rightCol.Item().Height(summaryHeight).Component(new BudgetSummaryComponent(summaryHeight));
                    });
                });
            });
        });
    }
}

public class MonthsHeaderComponent : IComponent
{
    public void Compose(IContainer container)
    {
        var months = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUNE", "JULY", "AUG", "SEPT", "OCT", "NOV", "DEC" };

        container.Row(row =>
        {
            for (int i = 0; i < months.Length; i++)
            {
                row.AutoItem().Text(months[i]);

                if (i < months.Length - 1)
                {
                    row.RelativeItem();
                }
            }
        });
    }
}

public class MainTitleComponent : IComponent
{
    public void Compose(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().AlignLeft().AlignMiddle().Text("Paycheck Budget".ToUpper()).FontSize(Theme.Fonts.HeaderSize).Black();

            row.ConstantItem(130f).AlignRight().AlignMiddle().Border(1).BorderColor(Theme.Colors.Primary).Column(col =>
            {
                col.Item().AlignCenter().Text("Date".ToUpper());
                col.Item().Extend();
            });
        });
    }
}

public class IncomeSectionComponent : IComponent
{
    private readonly float _height;
    public IncomeSectionComponent(float height) => _height = height;

    public void Compose(IContainer container)
    {
        Draw.Card(container, c => c.Column(col =>
        {
            int dataRows = 2;
            float rowHeight = Draw.CalculateRowHeight(_height, dataRows);

            col.Item().Height(rowHeight).Row(row =>
            {
                row.RelativeItem().AlignBottom().AlignRight().PaddingRight(70f).Text("Sources Of Income".ToUpper());
                row.ConstantItem(Layout.ColumnWidths.Default).AlignBottom().AlignCenter().Text("Budget".ToUpper());
                row.ConstantItem(Layout.ColumnWidths.Default).AlignBottom().AlignCenter().Text("Actual".ToUpper());
            });

            RenderIncomeRow(col, "Paycheck Amount", rowHeight, 110f);
            RenderIncomeRow(col, "Other Income Source", rowHeight, 125f);

            col.Item().Height(rowHeight).AlignBottom().Row(row =>
            {
                Draw.FooterLabel(row, "Total Income");
                Draw.FooterBox(row, rowHeight, Layout.ColumnWidths.Default, 4f);
                Draw.FooterBox(row, rowHeight, Layout.ColumnWidths.Default - 2f, 2f);
            });
        }));
    }

    private void RenderIncomeRow(ColumnDescriptor col, string label, float height, float lineLeftPadding)
    {
        col.Item().Height(height).Layers(layers =>
        {
            layers.PrimaryLayer().Row(row =>
            {
                row.RelativeItem().AlignBottom().Text(label.ToUpper());
                row.ConstantItem(Layout.ColumnWidths.Default).Padding(2f).Border(1).BorderColor(Theme.Colors.Primary);
                row.ConstantItem(Layout.ColumnWidths.Default).Padding(2f).Border(1).BorderColor(Theme.Colors.Primary);
            });

            layers.Layer()
                .AlignBottom()
                .PaddingBottom(2f)
                .Height(1f)
                .PaddingLeft(lineLeftPadding)
                .PaddingRight((2f * Layout.ColumnWidths.Default) + 2f) // Budget box (60pt) + Actual box (60pt) 
                .Background(Theme.Colors.Primary);
        });
    }
}

public class BudgetSummaryComponent : IComponent
{
    private readonly float _height;
    public BudgetSummaryComponent(float height) => _height = height;

    public void Compose(IContainer container)
    {
        Draw.Card(container, c => c.Column(col =>
        {

            int dataRows = 6;
            float rowHeight = Draw.CalculateRowHeight(_height, dataRows);

            col.Item().Height(rowHeight).AlignCenter().Text("Budget Summary".ToUpper()).FontSize(Theme.Fonts.TitleSize).ExtraBold();

            RenderSummaryItem(col, "Total Income", rowHeight, true);

            float minusOffset = 26f;
            col.Item().Height(rowHeight).AlignRight().AlignMiddle().PaddingRight(minusOffset).Text("Minus".ToUpper()).FontSize(Theme.Fonts.TitleSize);

            RenderSummaryItem(col, "Total Bills Paid", rowHeight, false);
            RenderSummaryItem(col, "Total Spending", rowHeight, false);
            RenderSummaryItem(col, "Total Savings", rowHeight, false);
            RenderSummaryItem(col, "Total Debt Paid", rowHeight, false);

            col.Item().Height(rowHeight).AlignBottom().Row(row =>
            {
                row.RelativeItem().AlignRight().PaddingRight(Layout.Spacing.FooterLabel).AlignMiddle().Text("Unused Money".ToUpper()).ExtraBold();
                row.ConstantItem(Layout.ColumnWidths.Summary).Height(rowHeight).Padding(2f).Border(Theme.Borders.Thick).BorderColor(Theme.Colors.Primary).Background(Theme.Colors.Gray);
            });
        }));
    }

    private void RenderSummaryItem(ColumnDescriptor col, string label, float height, bool isIncome)
    {
        col.Item().Height(height).AlignMiddle().Row(row =>
        {
            var textElement = row.RelativeItem().AlignRight().PaddingRight(Layout.Spacing.FooterLabel).AlignMiddle().Text(label.ToUpper()).NormalWeight();

            if (isIncome)
                textElement.Bold();

            row.ConstantItem(Layout.ColumnWidths.Summary).Height(height).Padding(2f).Border(1).BorderColor(Theme.Colors.Primary);
        });
    }
}

public static class DocumentSize
{
    public static readonly PageSize Letter = new(612f, 792f);
    public static readonly PageSize A4 = new(595f, 842f);
}

public static class SectionRatios
{
    public const float Half = 0.5f;

    public const float Savings = 0.3338f;
    public const float Debts = 0.3338f;
    public const float Summary = 0.3324f;
}

public static class Draw
{
    public static void RenderRow(ColumnDescriptor col, string label, float height, float linePadding, params float[] boxes)
    {
        RenderRow(col, label, height, linePadding, boxes, null);
    }

    public static void RenderRow(
        ColumnDescriptor col,
        string label,
        float height,
        float linePadding,
        float[] boxes,
        (float Horizontal, float Vertical)[]? boxPaddings)
    {
        col.Item().Height(height).Layers(layers =>
        {
            layers.PrimaryLayer().Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text(label);
                for (int i = 0; i < boxes.Length; i++)
                {
                    var currentPadding = (boxPaddings != null && i < boxPaddings.Length)
                        ? boxPaddings[i]
                        : (Horizontal: 2f, Vertical: 2f);

                    row.ConstantItem(boxes[i])
                        .PaddingHorizontal(currentPadding.Horizontal)
                        .PaddingVertical(currentPadding.Vertical)
                        .Border(Theme.Borders.Thin)
                        .BorderColor(Theme.Colors.Primary);
                }
            });
            layers.Layer().AlignBottom().Height(1f).PaddingRight(linePadding + 2f).Background(Theme.Colors.Primary);
        });
    }

    public static void Card(IContainer container, Action<IContainer> content)
    {
        container.Element(content);
    }

    public static void Title(RowDescriptor row, string title, Action<RowDescriptor> subHeaders)
    {
        row.RelativeItem().Text(title.ToUpper()).FontSize(Theme.Fonts.TitleSize);
        subHeaders(row);
    }

    public static void FooterLabel(RowDescriptor row, string label)
    {
        row.RelativeItem().AlignRight().AlignMiddle().PaddingRight(Layout.Spacing.FooterLabel).Text(label.ToUpper());
    }
    public static void FooterBox(RowDescriptor row, float height, float width, float padding)
    {
        row.ConstantItem(width)
           .Height(height)
           .PaddingVertical(2f)
           .PaddingRight(padding)
           .Border(Theme.Borders.Thin)
           .BorderColor(Theme.Colors.Primary);
    }

    public static float CalculateRowHeight(float totalHeight, int dataRows)
    {
        const int HeaderRows = 1;
        const int FooterRows = 1;

        return totalHeight / (HeaderRows + dataRows + FooterRows);
    }
}

public struct TrackerColumn
{
    public string Header { get; set; }
    public float Width { get; set; }
    public (float H, float V) Padding { get; set; }
}

public class TrackerComponent : IComponent
{
    private readonly string _title;
    private readonly int _dataRows;
    private readonly TrackerColumn[] _columns;
    private readonly float _height;
    private readonly string _footerLabel;
    private readonly float _footerWidth;
    private readonly float _footerPadding;

    public TrackerComponent(string title, int dataRows, TrackerColumn[] columns, float height, string footerLabel, float footerWidth, float footerPadding)
    {
        _title = title;
        _dataRows = dataRows;
        _columns = columns;
        _height = height;
        _footerLabel = footerLabel;
        _footerWidth = footerWidth;
        _footerPadding = footerPadding;
    }

    public void Compose(IContainer container)
    {
        Draw.Card(container, c => c.Column(col =>
        {
            float rowHeight = Draw.CalculateRowHeight(_height, _dataRows);

            col.Item().Height(rowHeight).Row(row =>
            {
                row.RelativeItem().Text(_title.ToUpper()).FontSize(Theme.Fonts.TitleSize);
                foreach (var colDef in _columns)
                {
                    row.ConstantItem(colDef.Width).AlignBottom().AlignCenter().Text(colDef.Header.ToUpper());
                }
            });

            for (int i = 0; i < _dataRows; i++)
            {
                float totalLinePadding = _columns.Sum(c => c.Width);

                float[] widths = _columns.Select(c => c.Width).ToArray();
                (float, float)[] paddings = _columns.Select(c => c.Padding).ToArray();

                Draw.RenderRow(col, string.Empty, rowHeight, totalLinePadding, widths, paddings);
            }

            if (!string.IsNullOrEmpty(_footerLabel))
            {
                col.Item().Height(rowHeight).AlignBottom().Row(row =>
                {
                    Draw.FooterLabel(row, _footerLabel);
                    Draw.FooterBox(row, rowHeight, _footerWidth, _footerPadding);
                });
            }
        }));
    }
}

