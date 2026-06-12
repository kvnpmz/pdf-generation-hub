using System;
using System.Diagnostics;
using System.IO;

public class ImageProcessor
{
    public bool IsEnabled { get; set; } = true; 
    private readonly string _etsyDir = "/home/kevin/z_ob/etsy";

    public void RunPipeline(string pdfPath)
    {
        if (!IsEnabled)
        {
            Console.WriteLine("[INFO] ImageProcessor is disabled. Skipping.");
            return;
        }

        string absPath = Path.GetFullPath(pdfPath);
        string? dir = Path.GetDirectoryName(absPath);
        string name = Path.GetFileNameWithoutExtension(absPath).Replace("_letter", "");

        if (dir == null) return;

        string outputDir = Path.Combine(dir, "images");

        if (!File.Exists(absPath))
        {
            Console.WriteLine($"[ERROR] File not found at: {name}");
            return;
        }

        Directory.CreateDirectory(outputDir);

        RunCommand("pdftoppm", $"-png -r 300 -singlefile \"{absPath}\" \"{Path.Combine(outputDir, name)}\"");

        string pngPath = Path.Combine(outputDir, name) + ".png";
        Console.WriteLine($"[DEBUG] Converting PDF to PNG using prefix: {pngPath}");

        if (File.Exists(pngPath))
        {
            CompositeImages(pngPath, outputDir, name);
        }
        else
        {
            Console.WriteLine($"[ERROR] PNG conversion failed. File {pngPath} does not exist.");
        }
    }

    private void CompositeImages(string pngPath, string outputDir, string name)
    {
        var bgs = new[] {
            new { Path = _etsyDir + "/ipad.png", Suffix = "ipad", Scale = "16.8%", Pos = "+69+112" },
            new { Path = _etsyDir + "/zoom.png", Suffix = "zoom", Scale = "20.0%", Pos = "+146+68" }
        };

        foreach (var bg in bgs)
        {
            string fileName = $"{name}_{bg.Suffix}.png";
            string cleanFileName = fileName.Replace("_letter", "");
            string outPath = Path.Combine(outputDir, cleanFileName);
            string args = $"convert \"{bg.Path}\" \\( \"{pngPath}\" -resize {bg.Scale} -geometry {bg.Pos} \\) -composite \"{outPath}\"";

            RunCommand("", args);
            Console.WriteLine($"[DEBUG] Saved: {outPath}");
        }
    }

    private void RunCommand(string cmd, string args)
    {
        try
        {
            string fullCommand = string.IsNullOrEmpty(cmd) ? args : $"{cmd} {args}";
            
            var startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{fullCommand}\"")
            {
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var proc = Process.Start(startInfo);
            proc?.WaitForExit();

            if (proc != null && proc.ExitCode != 0)
            {
                string error = proc.StandardError.ReadToEnd();
                Console.WriteLine($"[ERROR] Command '{cmd}' failed with code {proc.ExitCode}: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Exception running {cmd}: {ex.Message}");
        }
    }
}
