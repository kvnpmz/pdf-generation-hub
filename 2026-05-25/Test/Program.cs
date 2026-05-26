using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

#region Centralized Style & Fine-Tuning Engine
public static class LayoutTheme
{
    // Global Constraints
    public const float PageMarginY = 36f;
    public const float TargetPageHeight = 792f;

    // Typography & Spacing Scale (Tweak these to change text sizing globally)
    public static readonly ElementStyle HeaderStyle = new ElementStyle(MaxChar: 30, LineHeight: 10, Padding: 0, Border: 2);
    public static readonly ElementStyle SubheaderStyle = new ElementStyle(MaxChar: 20, LineHeight: 10, Padding: 0, Border: 0);
    public static readonly ElementStyle TitleStyle = new ElementStyle(MaxChar: 20, LineHeight: 10, Padding: 0, Border: 1);
    public static readonly ElementStyle SubtitleStyle = new ElementStyle(MaxChar: 20, LineHeight: 10, Padding: 0, Border: 0);
    public static readonly ElementStyle ItemStyle = new ElementStyle(MaxChar: 20, LineHeight: 10, Padding: 0, Border: 0);

    // Container Spacing (Tweak these to change structural gaps globally)
    public const float SectionPaddingY = 0f;
    public const float SectionBorderY = 0f;
    public const float ColumnPaddingY = 0f;
    public const float ColumnBorderY = 0f;
}

public record ElementStyle(int MaxChar, float LineHeight, float Padding, float Border);
#endregion

class Program
{
    static void Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Console.WriteLine("==================================================");
        Console.WriteLine("   QUESTPDF THEMED HEIGHT CALCULATION ENGINE     ");
        Console.WriteLine("==================================================\n");

        // 1. Create Header & Subheader using the LayoutTheme definitions
        var header = new TextElement("Company Annual Performance Report & Strategy Overview 2026", LayoutTheme.HeaderStyle);
        var subheader = new TextElement("Generated automatically by QuestPDF Layout Engine", LayoutTheme.SubheaderStyle);

        List<ColumnContainer> columns = new List<ColumnContainer>
        {
            BuildColumn1(),
            BuildColumn2(),
            BuildColumn3()
        };

        // 2. Render to PDF file
        string outputPath = "Test.pdf";
        
        var pdfDocument = new ReportDocument(header, subheader, columns);
        pdfDocument.GeneratePdf(outputPath);

        Console.WriteLine($"\nSuccess! PDF successfully rendered to: {Path.GetFullPath(outputPath)}");

        // 3. Calculate Header Heights
        float headerHeight = header.CalculateHeight();
        float subheaderHeight = subheader.CalculateHeight();

        Console.WriteLine($"[Header] Height: {headerHeight}pt (Lines: {header.CalculateLines()})");
        Console.WriteLine($"[Subheader] Height: {subheaderHeight}pt (Lines: {subheader.CalculateLines()})\n");

        float maxColumnHeight = 0;
        int columnCounter = 1;

        foreach (var col in columns)
        {
            float colHeight = col.CalculateTotalHeight();
            Console.WriteLine($"[Column {columnCounter}] Total Height: {colHeight}pt");

            if (colHeight > maxColumnHeight)
            {
                maxColumnHeight = colHeight;
            }
            columnCounter++;
        }

        // 5. Aggregate Total Page Height
        float totalOccupiedHeight = headerHeight + subheaderHeight + maxColumnHeight + LayoutTheme.PageMarginY;
        float remainingSpace = LayoutTheme.TargetPageHeight - totalOccupiedHeight;

        // 6. Print Summary Report
        Console.WriteLine("\n--------------------------------------------------");
        Console.WriteLine($"{"Fixed Page Margin:",-30} {LayoutTheme.PageMarginY}pt");
        Console.WriteLine($"{"Tallest Column Impact:",-30} {maxColumnHeight}pt");
        Console.WriteLine($"{"TOTAL OCCUPIED HEIGHT:",-30} {totalOccupiedHeight}pt / {LayoutTheme.TargetPageHeight}pt");
        Console.WriteLine("--------------------------------------------------");

        if (remainingSpace >= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"REMAINING VERTICAL SPACE:",-30} {remainingSpace}pt (Fits on page)");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{"REMAINING VERTICAL SPACE:",-30} {remainingSpace}pt (Content OVERFLOWS by {Math.Abs(remainingSpace)}pt)");
        }
        Console.ResetColor();
    }

    #region Mock Data Builders (Consuming the centralized theme)
    static ColumnContainer BuildColumn1()
    {
        var col = new ColumnContainer(LayoutTheme.ColumnPaddingY, LayoutTheme.ColumnBorderY);
        var sec = new Section(LayoutTheme.SectionPaddingY, LayoutTheme.SectionBorderY);

        sec.Title = new TextElement("Financial Metrics Summary", LayoutTheme.TitleStyle);
        sec.Subtitle = new TextElement("Q1-Q4 Overview", LayoutTheme.SubtitleStyle);
        sec.Items.Add(new TextElement("Revenue increased by 15% year-over-year", LayoutTheme.ItemStyle));
        sec.Items.Add(new TextElement("Net profit margins sustained at 22% average globally", LayoutTheme.ItemStyle));

        col.Sections.Add(sec);
        return col;
    }

    static ColumnContainer BuildColumn2()
    {
        var col = new ColumnContainer(LayoutTheme.ColumnPaddingY, LayoutTheme.ColumnBorderY);
        var sec = new Section(LayoutTheme.SectionPaddingY, LayoutTheme.SectionBorderY);

        sec.Title = new TextElement("Regional Operations & Logistics Development", LayoutTheme.TitleStyle);
        sec.Subtitle = new TextElement("Primary Focus Areas", LayoutTheme.SubtitleStyle);
        sec.Items.Add(new TextElement("Expansion into EMEA markets completed ahead of schedule", LayoutTheme.ItemStyle));

        col.Sections.Add(sec);
        return col;
    }

    static ColumnContainer BuildColumn3()
    {
        var col = new ColumnContainer(LayoutTheme.ColumnPaddingY, LayoutTheme.ColumnBorderY);
        var sec = new Section(LayoutTheme.SectionPaddingY, LayoutTheme.SectionBorderY);

        sec.Title = new TextElement("HR & Team Growth", LayoutTheme.TitleStyle);
        sec.Subtitle = new TextElement("Headcount", LayoutTheme.SubtitleStyle);
        sec.Items.Add(new TextElement("Hired 45 new engineers", LayoutTheme.ItemStyle));

        col.Sections.Add(sec);
        return col;
    }
    #endregion

    public static List<string> SplitText(string text, int max)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(text)) return result;
        string remainingText = text;

        while (remainingText.Length > max)
        {
            int lastSpace = remainingText.LastIndexOf(' ', max);
            
            int breakIndex = (lastSpace == -1) ? max : lastSpace;
            
            result.Add(remainingText.Substring(0, breakIndex).Trim());
            
            remainingText = remainingText.Substring(breakIndex).Trim();
        }
        
        if (!string.IsNullOrWhiteSpace(remainingText)) result.Add(remainingText);
        return result;
    }
}

#region Layout Model Classes
public class TextElement
{
    public string Text { get; set; }
    public ElementStyle Style { get; set; }

    public TextElement(string text, ElementStyle style)
    {
        Text = text ?? "";
        Style = style;
    }

    public int CalculateLines()
    {
        if (Text.Length == 0) return 1;

        // Ceiling math without floats floating point errors
        //int limit = Style.MaxChar < 1 ? 1 : Style.MaxChar;
        //int lines = (Text.Length + limit - 1) / limit;
        int lines = Program.SplitText(Text, Style.MaxChar).Count;

        return lines < 1 ? 1 : lines;
    }

    public float CalculateHeight()
    {
        int lines = CalculateLines();
        return (lines * Style.LineHeight);
    }
}

public class Section
{
    public TextElement? Title { get; set; }
    public TextElement? Subtitle { get; set; }
    public List<TextElement> Items { get; set; } = new List<TextElement>();
    public float PaddingY { get; set; }
    public float BorderY { get; set; }

    public Section(float paddingY, float borderY)
    {
        PaddingY = paddingY;
        BorderY = borderY;
    }

    public float CalculateTotalHeight()
    {
        float total = 0;

        if (Title != null)
            total += Title.CalculateHeight();

        if (Subtitle != null)
            total += Subtitle.CalculateHeight();

        foreach (var item in Items)
        {
            if (item != null)
            {
                total += item.CalculateHeight();
            }
        }

        return total + PaddingY;
    }
}

public class ColumnContainer
{
    public List<Section> Sections { get; set; } = new List<Section>();
    public float PaddingY { get; set; }
    public float BorderY { get; set; }

    public ColumnContainer(float paddingY, float borderY)
    {
        PaddingY = paddingY;
        BorderY = borderY;
    }

    public float CalculateTotalHeight()
    {
        float totalSectionsHeight = 0;

        foreach (var sec in Sections)
        {
            if (sec != null)
            {
                totalSectionsHeight += sec.CalculateTotalHeight();
            }
        }

        return totalSectionsHeight + PaddingY;
    }
}
#endregion

public class ReportDocument : IDocument
{
    private readonly TextElement _header;
    private readonly TextElement _subheader;
    private readonly List<ColumnContainer> _columns;

    public ReportDocument(TextElement header, TextElement subheader, List<ColumnContainer> columns)
    {
        _header = header;
        _subheader = subheader;
        _columns = columns;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.Letter);
            page.MarginVertical(LayoutTheme.PageMarginY);
            page.MarginHorizontal(50.4f); 
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(10));

            page.Content().Column(col =>
            {
                // Header
                if (_header != null)
                {
                    col.Item()
                       .Border(_header.Style.Border)
                       .Padding(_header.Style.Padding)
                       .Column(headerCol => 
                       {
                           // Split header text manually
                           var lines = Program.SplitText(_header.Text, _header.Style.MaxChar);
                           foreach (var line in lines)
                           {
                               headerCol.Item().Text(line).FontSize(12).Bold();
                           }
                       })
                       ;
                }

                // Subheader
                if (_subheader != null)
                {
                    col.Item()
                       .Padding(_subheader.Style.Padding)
                       .Column(subCol => 
                       {
                           var lines = Program.SplitText(_subheader.Text, _subheader.Style.MaxChar);
                           foreach (var line in lines)
                           {
                               subCol.Item().Text(line).FontSize(12).Italic().FontColor(Colors.Grey.Darken2);
                           }
                       });
                }
                // 3 Columns Row
                col.Item().Row(row =>
                {
                    foreach (var columnData in _columns)
                    {
                        row.RelativeItem().Padding(columnData.PaddingY).Column(innerCol =>
                        {
                            foreach (var section in columnData.Sections)
                            {
                                innerCol.Item().Border(section.BorderY).Padding(section.PaddingY).Column(secCol =>
                                {
                                    // Title
                                    if (section.Title != null)
                                    {
                                        var lines = Program.SplitText(section.Title.Text, section.Title.Style.MaxChar);
                                        foreach (var line in lines)
                                        {
                                            secCol.Item().Text(line).FontSize(12).Bold();
                                        }
                                    }

                                    // Subtitle
                                    if (section.Subtitle != null)
                                    {
                                        var lines = Program.SplitText(section.Subtitle.Text, section.Subtitle.Style.MaxChar);
                                        foreach (var line in lines)
                                        {
                                            secCol.Item().Text(line).FontSize(12).FontColor(Colors.Grey.Darken1);
                                        }
                                    }

                                    // Items (Bullet points)
                                    foreach (var item in section.Items)
                                    {
                                        var lines = Program.SplitText(item.Text, item.Style.MaxChar);
                                        
                                        // Wrap each list item in a sub-column to group bullet segments tightly
                                        secCol.Item().PaddingTop(item.Style.Padding).Column(itemCol =>
                                        {
                                            for (int i = 0; i < lines.Count; i++)
                                            {
                                                // Only add a bullet point symbol to the very first split string line
                                                string prefix = (i == 0) ? "• " : "  "; 
                                                itemCol.Item().Text($"{prefix}{lines[i]}").FontSize(12);
                                            }
                                        });
                                    }
                                });
                            }
                        });
                    }
                });
            });
        });
    }
}
