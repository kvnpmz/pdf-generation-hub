using System.Diagnostics;

public static class WeasyPrint
{
    public static async Task RenderAsync(string html, string outputBaseName)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "WeasyPrint");
        Directory.CreateDirectory(tempDir);

        var htmlFile = Path.Combine(tempDir, Guid.NewGuid() + ".html");
        var scriptFile = Path.Combine(tempDir, "render.py");

        await File.WriteAllTextAsync(htmlFile, html);
        await File.WriteAllTextAsync(scriptFile, 

"""
import sys
from pathlib import Path
from weasyprint import HTML, CSS

html_path = sys.argv[1] 
base_output_path = sys.argv[2]

# 1. Read the file content
html_content = Path(html_path).read_text(encoding='utf-8')

# 2. Use base_url to tell WeasyPrint that the 'string' is data, 
# and use the directory of the file as the base for relative resources (like CSS/images)
base_url = Path(html_path).parent.as_uri()

output_a4 = base_output_path + "_a4.pdf"
output_letter = base_output_path + "_letter.pdf"

# Render using the 'string' and 'base_url'
HTML(string=html_content, base_url=base_url).write_pdf(output_a4, pdf_forms=True)

letter_css = CSS(string='@page { size: Letter; }')
HTML(string=html_content, base_url=base_url).write_pdf(output_letter, pdf_forms=True, stylesheets=[letter_css])

print(f"SUCCESS: Created {output_a4} and {output_letter}")
""");

        var psi = new ProcessStartInfo
        {
            FileName = "/home/kevin/z_ob/venv/bin/python",
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        var fullOutputPath = Path.GetFullPath(outputBaseName); // Converts "editable" to "/home/kevin/app/editable"
        psi.ArgumentList.Add(scriptFile); 
        psi.ArgumentList.Add(htmlFile);
        psi.ArgumentList.Add(fullOutputPath);

        using var process = Process.Start(psi)!;

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
if (File.Exists(htmlFile)) File.Delete(htmlFile);
// ADD THIS:
if (!string.IsNullOrEmpty(stdout)) Console.WriteLine("PYTHON STDOUT: " + stdout);
if (!string.IsNullOrEmpty(stderr)) Console.WriteLine("PYTHON STDERR: " + stderr);

        if (process.ExitCode != 0)
            throw new Exception(stderr);
    }
}
