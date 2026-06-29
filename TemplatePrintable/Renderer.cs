using System.Diagnostics;
using System.Text;
using NLua;

public class Renderer : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        lua.DoString("local tl = require('tl'); tl.loader();");

        var config = (LuaTable)lua.DoString(
            $"return require('documents.{context.DocumentId}.config')"
        )[0];

        var template = config["template"]?.ToString()
            ?? throw new Exception("Missing template");

        var files = new[]
        {
            "init.tl",
            $"documents/{context.DocumentId}/config.tl",
            $"templates/{template}/render.tl"
        };

        await RunTlCheckAsync(files);

        var exports = (LuaTable)lua.DoString("return require('init')")[0];
        var render = (LuaFunction)exports["Render"];

        var result = (LuaTable)render.Call(context.DocumentId)[0];
        context.Html = result["html"]?.ToString() ?? string.Empty;

        context.OutputName = result["outputName"]?.ToString() ?? "output";
        string formattedHtml = HtmlFormatter.Beautify(context.Html);
        
        File.WriteAllText("preview.html", formattedHtml);
    }

    private static async Task RunTlCheckAsync(IEnumerable<string> files)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "tl",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        psi.ArgumentList.Add("check");

        foreach (var file in files)
            psi.ArgumentList.Add(file);

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start tl process");

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
            throw new Exception($"{stdout}\n{stderr}".Trim());
    }
}
