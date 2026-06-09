using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
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

    // 1. Load HTML
    string html = File.ReadAllText(htmlPath);

    // 2. CSS Discovery
    string dirInfo = Path.GetDirectoryName(htmlPath)!;
    string parentDate = Path.GetFileName(Path.GetDirectoryName(dirInfo))!;
    string folderName = Path.GetFileName(dirInfo)!;
    string fileName = Path.GetFileNameWithoutExtension(htmlPath);
    string searchPrefix = $"{parentDate}_{folderName}_{fileName}";
    string cssLibraryDir = "../layout_engine/";

    string[] matchingFiles = Directory.GetFiles(cssLibraryDir, $"{searchPrefix}_styles.css");

// 1. Load the CSS content
string cssContent = File.ReadAllText(matchingFiles[0]);

// 2. Create the fully formatted style block
string styleBlock = $"<style>\n{cssContent}\n</style>";

// 3. Perform the replacement
// This replaces your <style>{{style}}</style> (or whatever placeholder) 
// with the clean, complete style block.
if (html.Contains("<style>{{style}}</style>"))
{
    html = html.Replace("<style>{{style}}</style>", styleBlock);
}
else if (html.Contains("{{style}}"))
{
    html = html.Replace("{{style}}", cssContent);
}
else
{
    // Fallback: if no placeholder is found, inject before </head>
    html = html.Replace("</head>", $"{styleBlock}\n</head>");
}

    // 4. Save
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    File.WriteAllText(outputPath, html);
    }
}
