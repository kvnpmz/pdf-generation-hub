using Microsoft.Playwright;

public class GeneratePdf
{
    public async Task ConvertToPdfAsync(string html, string docName)
    {
        string docPath = $"output/{docName}"; 
        Directory.CreateDirectory(docPath);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        var formats = new[] { 
            new { Name = "letter", Format = PaperFormat.Letter }, 
            new { Name = "a4", Format = PaperFormat.A4 } 
        };

        foreach (var item in formats)
        {
            string fileName = item.Name == "a4" ? $"{docName}_{item.Name}.pdf" : $"{docName}.pdf";
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

