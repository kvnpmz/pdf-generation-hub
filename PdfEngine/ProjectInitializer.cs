using System.IO;

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
        string content = File.ReadAllText(dataFile);

        content = content.Replace(
            "id = \"example_checklist\"",
            $"id = \"{documentId}\""
        );

        var existing = File.ReadAllText(dataFile);

        if (existing != content)
        {
            File.WriteAllText(dataFile, content);
        }

        return documentId;
    }
}
