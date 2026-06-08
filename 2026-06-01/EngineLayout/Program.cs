using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NLua;
using System.Collections.Generic;

public interface IPdfExporter
{
    Task ExportAsync(string htmlContent, string baseFilename, string format);
}

public class PlaywrightExporter : IPdfExporter, IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

    private async Task EnsureInitializedAsync()
    {
        if (_browser != null) return;

        await _initLock.WaitAsync();
        try
        {
            if (_browser == null)
            {
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
                { 
                    Headless = true 
                });
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task ExportAsync(string htmlContent, string baseFilename, string format)
    {
        await EnsureInitializedAsync();
        
        await using var page = await _browser!.NewPageAsync();
        await page.SetContentAsync(htmlContent);

        string filename = $"{baseFilename.Replace(".pdf", "")}_{format}.pdf";

        await page.PdfAsync(new PagePdfOptions
        {
            Path = filename,
            Format = format,
            PrintBackground = true
        });

        Console.WriteLine($"Generated {filename}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null) await _browser.DisposeAsync();
        _playwright?.Dispose();
    }
}

public class Program
{
    private static readonly PlaywrightExporter _pdfExporter = new PlaywrightExporter();

    public static async Task Main()
    {
        using var lua = new Lua();
        
        lua.RegisterFunction("exportPdf", null, typeof(Program).GetMethod(nameof(ExportPdf)));
        
        lua.DoFile("main.lua");

        await _pdfExporter.DisposeAsync();
    }

    public static void ExportPdf(string htmlContent, string filename, string format)
    {
        try
        {
            _pdfExporter.ExportAsync(htmlContent, filename, format).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            var realException = ex.InnerException ?? ex;
            Console.WriteLine($"\n--- CRITICAL C# ERROR ---");
            Console.WriteLine($"Message: {realException.Message}");
            throw;
        }
    }
}
