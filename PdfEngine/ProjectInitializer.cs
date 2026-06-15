using System.IO;
using System.Text.RegularExpressions;

public class ProjectInitializer
{
    private const string OutputPath = "documents";

    public string CreateFolder(string documentId)
    {
        string destPath = Path.Combine(OutputPath, documentId);

        if (!Directory.Exists(destPath))
            Directory.CreateDirectory(destPath);

        string templatePath = Path.Combine(OutputPath, "example_checklist");

        if (!File.Exists(Path.Combine(destPath, "data.tl")))
        {
            File.Copy(
                Path.Combine(templatePath, "data.tl"),
                Path.Combine(destPath, "data.tl")
            );
        }

        if (!File.Exists(Path.Combine(destPath, "style.css")))
        {
            File.Copy(
                Path.Combine(templatePath, "style.css"),
                Path.Combine(destPath, "style.css")
            );
        }

        string dataFile = Path.Combine(destPath, "data.tl");
        string existing = File.ReadAllText(dataFile);

        string targetId = $"id = \"{documentId}\"";

        if (!existing.Contains(targetId))
        {
            existing = Regex.Replace(
                existing,
                @"id\s*=\s*"".*?""",
                targetId
            );

            File.WriteAllText(dataFile, existing);
        }

        return documentId;
    }
}
