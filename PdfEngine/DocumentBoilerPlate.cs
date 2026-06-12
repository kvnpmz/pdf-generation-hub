using System.IO;

public class DocumentBoilerplate
{
    private const string BaseDocumentsPath = "documents";

    public void CloneDocument(string sourceName, string destinationName)
    {
        string sourcePath = Path.Combine(BaseDocumentsPath, sourceName);
        string destPath = Path.Combine(BaseDocumentsPath, destinationName);

        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException($"Source '{sourceName}' not found.");

        if (Directory.Exists(destPath))
        {
            Console.WriteLine($"Directory '{destinationName}' already exists. Skipping copy.");
            return; 
        }

        Directory.CreateDirectory(destPath);

        string[] files = Directory.GetFiles(sourcePath);

        var excludedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
        { 
            ".pdf", ".png" 
        };

        foreach (string file in files)
        {
            string extension = Path.GetExtension(file);
            if (!excludedExtensions.Contains(extension))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destPath, fileName);
                
                File.Copy(file, destFile, true);
            }
        }
    }
}
