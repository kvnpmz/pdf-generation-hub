using System.IO;
using System.Text.RegularExpressions;

public class Initializer : IPipelineStep
{
    private const string OutputPath = "documents";

    public async Task ExecuteAsync(PipelineContext context)
    {
        string documentId = context.DocId;
        string destinationPath = Path.Combine(OutputPath, documentId);

        if (!Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);

        string examplePath = Path.Combine(OutputPath, "example_checklist");
        string configFileDestination = Path.Combine(destinationPath, "config.tl");

        if (!File.Exists(configFileDestination))
        {
            using (var sourceStream = File.OpenRead(Path.Combine(examplePath, "config.tl")))
            using (var destinationStream = File.Create(configFileDestination))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }
        }

        string existing = await File.ReadAllTextAsync(configFileDestination);
        string targetId = $"id = \"{documentId}\"";

        if (!existing.Contains(targetId))
        {
            existing = Regex.Replace(
                existing,
                @"id\s*=\s*"".*?""",
                targetId
            );

            await File.WriteAllTextAsync(configFileDestination, existing);
        }
    }
}
