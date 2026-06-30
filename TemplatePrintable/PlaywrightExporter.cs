using Microsoft.Playwright;

public class PlaywrightExporter : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        string docPath = $"output/{context.DocumentId}"; 
        Directory.CreateDirectory(docPath);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(context.Html);

        var formats = new[] { 
            new { Format = PaperFormat.Letter, Suffix = ""}, 
            new { Format = PaperFormat.A4, Suffix = "_a4" } 
        };

        foreach (var item in formats)
        {
            string fileName = $"{context.OutputName}{item.Suffix}.pdf";
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
