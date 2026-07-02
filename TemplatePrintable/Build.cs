using System.Diagnostics;
using System.Text;
using NLua;

public class Build : IStep
{
    public async Task ExecuteAsync(Context context)
    {
        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        lua.DoString("local tl = require('tl'); tl.loader();");

        var config = (LuaTable)lua.DoString(
            $"return require('documents.{context.DocumentId}.config')"
        )[0];

        var template = config["template"]?.ToString()
            ?? throw new Exception("Missing template");

        context.IsEditable = config["is_editable"] is bool b && b;

        var files = new[]
        {
            "init.tl",
            Path.Combine("documents", context.DocumentId, "config.tl"),
            Path.Combine("templates", template, "render.tl")
        };

        await RunTlCheckAsync(files);

        var exports = (LuaTable)lua.DoString("return require('init')")[0];
        var render = (LuaFunction)exports["Render"];

        var result = (LuaTable)render.Call(context.DocumentId)[0];
        context.Html = result["html"]?.ToString() ?? string.Empty;

        context.OutputName = result["outputName"]?.ToString() ?? "output";
        string formattedHtml = Format.Beautify(context.Html);
        
        var htmlPath = Path.Combine(context.OutputDirectory, $"{context.OutputName}.html");
        File.WriteAllText(htmlPath, formattedHtml);
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

        using var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.Error.WriteLine(e.Data);
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new Exception($"tl check failed (exit {process.ExitCode})");
    }
}
