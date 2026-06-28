using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Initializer : IPipelineStep
{
    private const string BaseProjectName = "kidney_foodlist";
    private const string DocsDirectory = "documents";
    private const string TemplatesDirectory = "templates";

    public async Task ExecuteAsync(PipelineContext context)
    {
        string projectPath = Path.Combine(DocsDirectory, context.DocumentId);
        // If the project exists, we skip scaffolding (Idempotency)
        if (Directory.Exists(projectPath)) return;

        string baseConfigContent = await File.ReadAllTextAsync(Path.Combine(DocsDirectory, BaseProjectName, "config.tl"));
        string? templateName = Regex.Match(
            baseConfigContent,
            @"template\s*=\s*""([^""]+)"""
        ).Groups[1].Value;

        string templateSourcePath = Path.Combine(TemplatesDirectory, templateName);

        if (!Directory.Exists(templateSourcePath))
        {
            throw new DirectoryNotFoundException($"Template does not exist: {templateSourcePath}");
        }

        string baseDocumentPath = Path.Combine(DocsDirectory, BaseProjectName);
        CopyDirectory(baseDocumentPath, projectPath);


        string projectConfigPath = (Path.Combine(projectPath, "config.tl"));
        string projectConfigContent = await File.ReadAllTextAsync(projectConfigPath);

        bool baseHasId = Regex.IsMatch(baseConfigContent, @"id\s*=\s*""[^""]*""");
        var ReturnPattern = @"return\s*\{\s*";
        var IdPattern = @"id\s*=\s*"".*?""";

        if (!baseHasId)
        {
            projectConfigContent = Regex.Replace(
                projectConfigContent,
                ReturnPattern,
                match => match.Value + $"id = \"{context.DocumentId}\",{Environment.NewLine}    "
            );
        }
        else
        {
            projectConfigContent = Regex.Replace(projectConfigContent, IdPattern, $"id = \"{context.DocumentId}\"");
        }

        await File.WriteAllTextAsync(projectConfigPath, projectConfigContent);
        
        Console.WriteLine($"[SUCCESS] Project {context.DocumentId} scaffolded.");
    }

    private void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.GetFiles(source))
        {
            string fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(destination, fileName), true);
        }
    }
}
