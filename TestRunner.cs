using System.Threading.Tasks;
using TemplatePrintable.Core;

public class TestRunner
{
    public static async Task Run()
    {
        var watcher = new Watcher();

        var runner = new Runner(new Flow()); 
        await runner.BuildAsync();

        var pdfExists = File.Exists(
                Path.Combine(Paths.RootPath, "output", "grocery_list.pdf"));

        if (!pdfExists)
            throw new Exception("PDF was not generated");

        Console.WriteLine("Integration test passed");
    }
}
