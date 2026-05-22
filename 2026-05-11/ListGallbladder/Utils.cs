using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

public class LayoutCalculator
{
    // Constants for A4 (or Letter adjustments)
    private const double MARGIN = 72;
    private const double H1_HEIGHT = 36;
    private const double H1_MARGIN = 10;
    private const double H2_HEIGHT = 22;
    private const double ITEM_HEIGHT = 16;
    private const double SECTION_BORDER = 1.3;
    private const double SECTION_PADDING = 5;

    public async Task<Dictionary<string, object>> CalculateDynamicMargins(
        string htmlContent,
        double pageHeight,
        double pageWidth,
        double scale)
    {
        double containerHeight = pageHeight - MARGIN - H1_HEIGHT - H1_MARGIN - 1;

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(htmlContent));

        var columns = document.QuerySelectorAll(".column");

        var margins = new List<double>();

        foreach (var col in columns)
        {
            var sections = col.QuerySelectorAll(".section");
            int numSections = sections.Length;

            if (numSections <= 1)
            {
                margins.Add(0);
                continue;
            }

            int h2Count = col.QuerySelectorAll("h2").Length;
            int itemCount = col.QuerySelectorAll(".item").Length;

            double usedSpace =
                (h2Count * H2_HEIGHT * scale) +
                (itemCount * ITEM_HEIGHT * scale);

            double leftoverSpace = containerHeight - usedSpace;

            double marginPer = leftoverSpace / (numSections - 1);

            margins.Add(Math.Round(Math.Max(0, marginPer), 4));
        }

        return new Dictionary<string, object>
        {
            ["h1_height"] = H1_HEIGHT,
            ["h1_margin"] = H1_MARGIN,
            ["h2_height"] = H2_HEIGHT * scale,
            ["item_height"] = ITEM_HEIGHT * scale,
            ["container_height"] = containerHeight,
            ["section_border"] = SECTION_BORDER,
            ["section_padding"] = SECTION_PADDING,
            ["page_height"] = pageHeight,
            ["page_width"] = pageWidth,
            ["margins"] = margins
        };
    }
}
