using Microsoft.Playwright;

public class Exporter : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        string html = context.Html;
        string docId = context.DocumentId;
        string outputName = context.OutputName;

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

