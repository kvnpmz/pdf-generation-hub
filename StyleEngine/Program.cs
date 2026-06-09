using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string htmlTemplatePath = "Templates/invoice.html";

        string[] cssFiles =
        {
            "Styles/base.css",
            "Styles/theme.css",
            "Styles/layout.css"
        };

        string outputPath = "Output/invoice.html";

        // Read HTML template
        string html = File.ReadAllText(htmlTemplatePath);

        // Combine all CSS files
        StringBuilder cssBuilder = new StringBuilder();

        foreach (string cssFile in cssFiles)
        {
            if (!File.Exists(cssFile))
            {
                Console.WriteLine($"Warning: {cssFile} not found.");
                continue;
            }

            cssBuilder.AppendLine(File.ReadAllText(cssFile));
            cssBuilder.AppendLine();
        }

        // Create inline style block
        string styleBlock =
$@"<style>
{cssBuilder}
</style>";

        // Option 1: Use a placeholder
        if (html.Contains("{{styles}}"))
        {
            html = html.Replace("{{styles}}", styleBlock);
        }
        else
        {
            // Option 2: Inject before </head>
            html = html.Replace("</head>",
                $"{Environment.NewLine}{styleBlock}{Environment.NewLine}</head>");
        }

        // Ensure output directory exists
        string? outputDir = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Write final HTML
        File.WriteAllText(outputPath, html);

        Console.WriteLine($"Generated: {outputPath}");
    }
}
