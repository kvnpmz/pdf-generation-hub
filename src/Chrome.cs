using Microsoft.Playwright;

public class Chrome : IPdfExport
{
    private const string SystemChromiumPath = "/usr/bin/chromium";

    // Prefer Playwright's dedicated headless shell: it needs ~75 OS tasks instead of
    // ~130 for a full Chromium, which matters in containers with a low pids cgroup cap.
    // Override with CHROME_EXECUTABLE when a specific binary is required.
    private static string ResolveExecutable()
    {
        var overridePath = Environment.GetEnvironmentVariable("CHROME_EXECUTABLE");
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
            return overridePath;

        var browsersPath =
            Environment.GetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH")
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache",
                "ms-playwright"
            );

        if (Directory.Exists(browsersPath))
        {
            foreach (
                string dir in Directory
                    .GetDirectories(browsersPath, "chromium_headless_shell-*")
                    .OrderByDescending(d => d, StringComparer.OrdinalIgnoreCase)
            )
            {
                string shell = Path.Combine(dir, "chrome-headless-shell-linux64", "chrome-headless-shell");
                if (File.Exists(shell))
                    return shell;
            }
        }

        return SystemChromiumPath;
    }

    public async Task ExecuteAsync(Context context)
    {
        var launchOptions = new BrowserTypeLaunchOptions
        {
            ExecutablePath = ResolveExecutable(),
            Headless = true,
            Args = new[] { "--disable-dev-shm-usage" }
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
