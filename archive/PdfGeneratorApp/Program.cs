using System;
using System.IO;
using NLua;
using Microsoft.Playwright;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();

        var renderer = new PlaywrightRenderer(playwright, browser);

        using var lua = new Lua();
        lua.LoadCLRPackage();
        lua.DoFile("main.lua");

        var doc = lua["doc"] as LuaTable;
        var style = lua["style"] as LuaTable;

        string type = doc?["type"]?.ToString() ?? "default";
        string title = doc?["title"]?.ToString() ?? "Untitled";

        string templatePath = type switch
        {
            "invoice" => "templates/pdf1.html",
            "report"  => "templates/pdf2.html",
            _         => "templates/pdf3.html"
        };

        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Error: Template not found at {templatePath}");
            return;
        }

        string html = await File.ReadAllTextAsync(templatePath);

        string cssVars = "";
        if (style != null && style["titleColor"] != null)
        {
            cssVars = $"--title-color: {style["titleColor"]};";
        }

        html = html.Replace("{{CSS_VARS}}", cssVars);
        html = html.Replace("{{TITLE}}", title);

        // 3. Render PDF using the reused browser
        await renderer.RenderPdf(html, $"{type}.pdf");
        Console.WriteLine($"PDF generated: {type}.pdf");
    }
}

public class PlaywrightRenderer : IAsyncDisposable
{
    private readonly IPlaywright _playwright;
    private readonly IBrowser _browser;

    public PlaywrightRenderer(IPlaywright playwright, IBrowser browser)
    {
        _playwright = playwright;
        _browser = browser;
    }

    public async Task RenderPdf(string htmlContent, string outputPath)
    {
        var page = await _browser.NewPageAsync();
        
        await page.SetContentAsync(htmlContent);
        await page.PdfAsync(new PagePdfOptions { Path = outputPath });
        
        await page.CloseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }
}
