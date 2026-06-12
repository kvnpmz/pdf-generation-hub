using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    // The Cache: Persists across the entire execution of the program
    private static Dictionary<string, string> _cssCache = new();

    static void Main(string[] args)
    {
        // 1. Load Config
        string yamlContent = File.ReadAllText("config.yaml");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var config = deserializer.Deserialize<Dictionary<string, dynamic>>(yamlContent);

        string sourceDir = config["settings"]["source_directory"];
        string outputDir = config["settings"]["output_directory"];
        string pattern = config["settings"]["search_pattern"];

        // 2. Discover Files
        var htmlFiles = Directory.GetFiles(sourceDir, pattern, SearchOption.AllDirectories);
        Console.WriteLine($"Found {htmlFiles.Length} files to process.");

        // 3. Process each file
        foreach (var htmlPath in htmlFiles)
        {
            ProcessFile(htmlPath, sourceDir, outputDir);
        }

        Console.WriteLine("Build Complete.");
    }

static void ProcessFile(string htmlPath, string sourceDir, string outputDir)
{
    string relativePath = Path.GetRelativePath(sourceDir, htmlPath);
    string outputPath = Path.Combine(outputDir, relativePath);

    // 1. Get Directory Information safely
    string? htmlDir = Path.GetDirectoryName(htmlPath);
    if (htmlDir == null) return;

    string? parentDir = Path.GetDirectoryName(htmlDir);
    string parentDate = Path.GetFileName(parentDir) ?? "unknown_date";
    string folderName = Path.GetFileName(htmlDir) ?? "unknown_folder";
    string fileName = Path.GetFileNameWithoutExtension(htmlPath);
    
    string searchPrefix = $"{parentDate}_{folderName}_{fileName}";
    string cssLibraryDir = "../layout_engine/";

    // 2. Find CSS
    string[] matchingFiles = Directory.GetFiles(cssLibraryDir, $"{searchPrefix}_styles.css");
    
    if (matchingFiles.Length == 0)
    {
        Console.WriteLine($"Warning: No CSS found for {fileName}");
        return;
    }

    // 3. Get CSS content
    string cssContent = GetCssContent(matchingFiles[0]);

    // 4. Load HTML and Perform Replacement
    string html = File.ReadAllText(htmlPath);
    string styleBlock = $"<style>\n{cssContent}\n</style>";

    if (html.Contains("<style>{{style}}</style>"))
        html = html.Replace("<style>{{style}}</style>", styleBlock);
    else if (html.Contains("{{style}}"))
        html = html.Replace("{{style}}", cssContent);
    else
        html = html.Replace("</head>", $"{styleBlock}\n</head>");

    // 5. Save (Null-safe directory creation)
    string? outputDirName = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrEmpty(outputDirName))
    {
        Directory.CreateDirectory(outputDirName);
    }
    
    File.WriteAllText(outputPath, html);
}

    // Helper method to handle caching
    static string GetCssContent(string cssPath)
    {
        if (!_cssCache.TryGetValue(cssPath, out string? content))
        {
            content = File.ReadAllText(cssPath);
            _cssCache[cssPath] = content;
        }
        return content;
    }
}
