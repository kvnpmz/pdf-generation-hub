using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Initializer : IPipelineStep
{
    private const string BaseTemplate = "monthly_template";
    private const string DocsDir = "documents";
    private const string TemplatesDir = "templates";

    public async Task ExecuteAsync(PipelineContext context)
    {
        string docPath = Path.Combine(DocsDir, context.DocId);

        // If the project exists, we skip scaffolding (Idempotency)
        if (Directory.Exists(docPath)) return;

        Directory.CreateDirectory(docPath);
        string templateDest = Path.Combine(TemplatesDir, context.DocId);
        Directory.CreateDirectory(templateDest);

        CopyDirectory(Path.Combine(DocsDir, BaseTemplate), docPath);
        CopyDirectory(Path.Combine(TemplatesDir, BaseTemplate), templateDest);

        string configPath = Path.Combine(docPath, "config.tl");
        string configContent = await File.ReadAllTextAsync(configPath);

        configContent = Regex.Replace(configContent, @"id\s*=\s*"".*?""", $"id = \"{context.DocId}\"");

        await File.WriteAllTextAsync(configPath, configContent);
        
        Console.WriteLine($"[SUCCESS] Project {context.DocId} scaffolded.");
    }

    private void CopyDirectory(string source, string dest)
    {
        foreach (var file in Directory.GetFiles(source))
        {
            string fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(dest, fileName), true);
        }
    }
}
