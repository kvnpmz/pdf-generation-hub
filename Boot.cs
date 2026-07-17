using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Boot : IStep
{
    private static readonly string DocsDirectory = Path.Combine(Paths.RootPath, "documents");
    private static readonly string TemplatesDirectory = Path.Combine(Paths.RootPath, "templates");

    public async Task ExecuteAsync(Context context)
    {
        string projectPath = Path.Combine(DocsDirectory, context.DocumentId);
        // If the project exists, we skip scaffolding (Idempotency)
        if (Directory.Exists(projectPath))
            return;

        Watcher.BootState.SuppressEvents = true;

        try
        {
            var baseProjectName = context.BaseProjectName;
            string baseConfigContent = await File.ReadAllTextAsync(
                Path.Combine(DocsDirectory, baseProjectName, "config.tl")
            );
            string? templateName = Regex
                .Match(baseConfigContent, @"template\s*=\s*""([^""]+)""")
                .Groups[1]
                .Value;

            string templateSourcePath = Path.Combine(TemplatesDirectory, templateName);

            if (!Directory.Exists(templateSourcePath))
            {
                throw new DirectoryNotFoundException(
                    $"Template does not exist: {templateSourcePath}"
                );
            }

            string baseDocumentPath = Path.Combine(DocsDirectory, baseProjectName);
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
                    match =>
                        match.Value + $"id = \"{context.DocumentId}\",{Environment.NewLine}    "
                );
            }
            else
            {
                projectConfigContent = Regex.Replace(
                    projectConfigContent,
                    IdPattern,
                    $"id = \"{context.DocumentId}\""
                );
            }

            await File.WriteAllTextAsync(projectConfigPath, projectConfigContent);

            Console.WriteLine($"[SUCCESS] Project {context.DocumentId} scaffolded.");
        }
        finally
        {
            Watcher.BootState.SuppressEvents = false;
        }
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
