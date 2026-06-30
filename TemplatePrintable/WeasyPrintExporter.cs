using System.Diagnostics;

public class WeasyPrintExporter : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        Console.WriteLine("Executing WeasyPrint strategy...");
        var tempDir = Path.Combine(Path.GetTempPath(), "WeasyPrint");
        Directory.CreateDirectory(tempDir);

        var htmlFile = "preview.html";
        var scriptFile = Path.Combine(tempDir, Guid.NewGuid() + ".py");

        var fullOutputPath = $"output/{context.DocumentId}";
        await File.WriteAllTextAsync(scriptFile, 

"""
import sys
from pathlib import Path
from weasyprint import HTML, CSS

html_path = Path(sys.argv[1])
output_directory = Path(sys.argv[2])
document_id = sys.argv[3]

output_a4 = output_directory / f"{document_id}_editable_a4.pdf"
output_letter = output_directory / f"{document_id}_editable.pdf"

HTML(filename=html_path).write_pdf(output_a4, pdf_forms=True)

letter_css = CSS(string='@page { size: Letter; }')
HTML(filename=html_path).write_pdf(output_letter, pdf_forms=True, stylesheets=[letter_css])
""");

        var psi = new ProcessStartInfo
        {
            FileName = "/home/kevin/z_ob/venv/bin/python",
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add(scriptFile);
        psi.ArgumentList.Add(htmlFile);
        psi.ArgumentList.Add(fullOutputPath);
        psi.ArgumentList.Add(context.DocumentId);

        using var process = Process.Start(psi)!;

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        File.Delete(scriptFile);

        if (process.ExitCode != 0)
            throw new Exception(stderr);
    }
}
