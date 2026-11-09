using System.Diagnostics;

public class Image : IStep
{
    private static readonly string _assetsDir = Path.Combine(Paths.RootPath, "assets");

    // Ghostscript's bundled sRGB profile ships with the runtime's data files;
    // keep this in sync with the installed ghostscript version (see libgs.so.10.xx).
    private const string IccProfilePath = "/usr/share/ghostscript/10.08.0/iccprofiles/srgb.icc";

    public async Task ExecuteAsync(Context context)
    {
        if (!context.EnableImages)
        {
            Console.WriteLine("[INFO] ImageConverter is disabled. Skipping.");
            return;
        }

        string[] pdfFiles = Directory.GetFiles(context.OutputDirectory, "*.pdf");

        foreach (string filePath in pdfFiles)
        {
            ProcessPdfToImages(filePath);
        }
    }

    public void ProcessPdfToImages(string pdfPath)
    {
        /*
        if (pdfPath.Contains("editable", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("[INFO] Pdf is editable. Skipping.");
            return;
        }
        */

        string absPath = Path.GetFullPath(pdfPath);
        string? dir = Path.GetDirectoryName(absPath);
        string name = Path.GetFileNameWithoutExtension(absPath);

        if (dir == null)
            return;

        string outputDir = Path.Combine(dir, "images");

        if (!File.Exists(absPath))
        {
            Console.WriteLine($"[ERROR] File not found at: {absPath}");
            return;
        }

        Directory.CreateDirectory(outputDir);

        string pngPath = Path.Combine(outputDir, name) + ".png";

        string arguments =
            $"-density 300 \"{absPath}\" -colorspace sRGB -background white -alpha remove -type truecolor -profile \"{IccProfilePath}\" \"{pngPath}\"";

        if (!RunCommand("magick", arguments) || !File.Exists(pngPath))
        {
            // Never composite or report success from a stale PNG: abort this file instead.
            Console.WriteLine($"[ERROR] PDF to PNG conversion failed for {absPath}. Skipping images.");
            return;
        }

        Console.WriteLine($"[SUCCESS] Converting PDF to PNG: {pngPath}");
        CompositeImages(pngPath, outputDir, name);
    }

    private void CompositeImages(string pngPath, string outputDir, string name)
    {
        if (name.EndsWith("_a4", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var bgs = new[]
        {
            new
            {
                Path = _assetsDir + "/ipad.png",
                Suffix = "ipad",
                Scale = "16.8%",
                Pos = "+69+112",
            },
            new
            {
                Path = _assetsDir + "/zoom.png",
                Suffix = "zoom",
                Scale = "20.0%",
                Pos = "+146+58",
            },
        };

        foreach (var bg in bgs)
        {
            string fileName = $"{name}_{bg.Suffix}.png";
            string cleanFileName = fileName;
            string outPath = Path.Combine(outputDir, cleanFileName);
            string args =
                $"\"{bg.Path}\" \\( \"{pngPath}\" -resize {bg.Scale} -geometry {bg.Pos} \\) -composite \"{outPath}\"";

            if (RunCommand("convert", args))
                Console.WriteLine($"[SUCCESS] Saved: {fileName}");
            else
                Console.WriteLine($"[ERROR] Failed to create composite: {fileName}");
        }
    }

    private bool RunCommand(string cmd, string args)
    {
        try
        {
            string fullCommand = string.IsNullOrEmpty(cmd) ? args : $"{cmd} {args}";

            var startInfo = new ProcessStartInfo("/bin/bash", $"-c \"{fullCommand}\"")
            {
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var proc = Process.Start(startInfo);

            if (proc == null)
            {
                Console.WriteLine($"[ERROR] Could not start '{cmd}'.");
                return false;
            }

            // Drain stderr before waiting to avoid deadlocking on a full pipe buffer.
            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                Console.WriteLine(
                    $"[ERROR] Command '{cmd}' failed with code {proc.ExitCode}: {error.Trim()}"
                );
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Exception running {cmd}: {ex.Message}");
            return false;
        }
    }
}
