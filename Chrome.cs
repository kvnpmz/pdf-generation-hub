using Microsoft.Playwright;

public class Chrome : IPdfRender
{
    public async Task ExecuteAsync(Context context)
    {
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
            string outputPath = Path.GetFullPath(
                    Path.Combine(Paths.RootPath, context.OutputDirectory, fileName));

            await page.PdfAsync(new()
            {
                Path = outputPath,
                Format = item.Format,
                PrintBackground = true
            });

            Console.WriteLine($"Generated {fileName}");
            Console.WriteLine($"Full path: {outputPath}");
        }
    }
}
