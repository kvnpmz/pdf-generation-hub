using Microsoft.Playwright;

public class Chrome : IPdfExport
{
    public async Task ExecuteAsync(Context context)
    {
        var launchOptions = new BrowserTypeLaunchOptions
        {
            ExecutablePath = "/usr/bin/chromium",
            Headless = true
        };

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(context.Html);

        var formats = new[]
        {
            new { Format = PaperFormat.Letter, Suffix = "" },
            // new { Format = PaperFormat.A4, Suffix = "_a4" },
        };

        foreach (var item in formats)
        {
            string fileName = $"{context.OutputName}{item.Suffix}.pdf";
            string outputPath = Path.GetFullPath(Path.Combine(context.OutputDirectory, fileName));

            await page.PdfAsync(
                    new()
                    {
                    Path = outputPath,
                    Format = item.Format,
                    PrintBackground = true,
                    }
                    );

            var rootName = Path.GetFileName(Paths.RootPath);
            var index = outputPath.IndexOf(rootName, StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                Console.WriteLine($"Generated {outputPath[index..]}");
            }
            else
            {
                Console.WriteLine($"Generated {outputPath}");
            }
        }
    }
}
