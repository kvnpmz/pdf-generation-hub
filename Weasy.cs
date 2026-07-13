using System.Diagnostics;

public class Weasy : IPdfEngine
{
    private static readonly string htmlFile = Path.Combine(Paths.RootPath, "preview.html");

    public async Task ExecuteAsync(Context context)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "WeasyPrint");
        Directory.CreateDirectory(tempDir);

        var scriptFile = Path.Combine(tempDir, Guid.NewGuid() + ".py");

        await File.WriteAllTextAsync(scriptFile,

"""
import sys
from pathlib import Path
from weasyprint import HTML, CSS

html_path = Path(sys.argv[1])
output_directory = Path(sys.argv[2])
output_name = sys.argv[3]

output_a4 = output_directory / f"{output_name}_editable_a4.pdf"
output_letter = output_directory / f"{output_name}_editable.pdf"

HTML(filename=html_path).write_pdf(output_a4, pdf_forms=True)

letter_css = CSS(string='@page { size: Letter; }')
HTML(filename=html_path).write_pdf(output_letter, pdf_forms=True, stylesheets=[letter_css])
print(f"Generated {output_a4}");       
print(f"Generated {output_letter}");       
""");

        var psi = new ProcessStartInfo
        {
            FileName = Paths.PythonPath,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add(scriptFile);
        psi.ArgumentList.Add(htmlFile);
        psi.ArgumentList.Add(context.OutputDirectory);
        psi.ArgumentList.Add(context.OutputName);

        using var process = Process.Start(psi)!;

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        Console.WriteLine(stdout);

        await process.WaitForExitAsync();

        File.Delete(scriptFile);

        if (process.ExitCode != 0)
            throw new Exception(stderr);

    }
}
