using Microsoft.Playwright;

public class GeneratePdf
{
    public async Task ConvertToPdfAsync(string html, string docId, string outputName)
    {
        string docPath = $"output/{docId}"; 
        Directory.CreateDirectory(docPath);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        var formats = new[] { 
            new { Format = PaperFormat.Letter, Suffix = ""}, 
            new { Format = PaperFormat.A4, Suffix = "_a4" } 
        };

        foreach (var item in formats)
        {
            string fileName = $"{outputName}{item.Suffix}.pdf";
            await page.PdfAsync(new()
            {
                Path = $"{docPath}/{fileName}",
                Format = item.Format,
                PrintBackground = true
            });
            Console.WriteLine($"Generated {fileName}");       
        }
    }
}

