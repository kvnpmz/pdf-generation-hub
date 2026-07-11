using System.Diagnostics;
using System.Text;
using NLua;

public class RenderResult
{
    public string Html { get; set; } = "";
    public string OutputName { get; set; } = "output";
}

public class Build : IStep
{
    public async Task ExecuteAsync(Context context)
    {
        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;

        var luaPath =
            $"package.path = package.path .. ';{Path.Combine(Paths.RootPath, "?.tl")}' " +
            $".. ';{Path.Combine(Paths.RootPath, "?/init.tl")}'";

        lua.DoString(luaPath);
        lua["ROOT_PATH"] = Paths.RootPath;

        lua.DoString("local tl = require('tl'); tl.loader();");

        var config = (LuaTable)lua.DoString(
                $"return require('documents.{context.DocumentId}.config')"
                )[0];

        context.Config = config;

        var template = config["template"]?.ToString()
            ?? throw new Exception("Missing template");

        context.IsEditable = config["is_editable"] is bool b && b;

        var renderPath = Path.Combine(
                Paths.RootPath,
                "templates",
                template,
                "render"
                );

        if (File.Exists(renderPath + ".tl"))
        {
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
        }
        else if (File.Exists(renderPath + ".cs"))
        {
    if (context.Flow != null && context.Flow.TryGetRenderer(template, out var renderer))
    {
        var result = renderer!.Render(config);
        
        if (result == null) throw new Exception("Renderer returned null");

        var htmlContent = DocumentGenerator.GenerateDocument(new Dictionary<string, object> {
            { "renderTemplate", (Func<object, string>)(_ => result.Html) },
            { "config", config }
        });

        context.Html = StyleApplier.Apply(htmlContent, config);
        context.OutputName = result.OutputName;
    }
    else
    {
        throw new Exception($"Could not find registered renderer for '{template}' in Flow.");
    }
        }
        else
        {
            throw new Exception($"No render.tl or render.cs found for template '{template}'");
        }

        string formattedHtml = Format.Beautify(context.Html);

        var htmlPath = Path.Combine(Paths.RootPath, context.OutputDirectory, $"{context.OutputName}.html");
        var previewPath = Path.Combine(Paths.RootPath, "preview.html");
        File.WriteAllText(htmlPath, formattedHtml);
        File.WriteAllText(previewPath, formattedHtml);
    }

    private static async Task RunTlCheckAsync(IEnumerable<string> files)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "tl",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Paths.RootPath
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

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var sb = new System.Text.StringBuilder(value.Length);
        bool nextUpper = true;

        foreach (char c in value)
        {
            if (c == '_')
            {
                nextUpper = true;
            }
            else
            {
                if (nextUpper)
                {
                    sb.Append(char.ToUpper(c));
                    nextUpper = false;
                }
                else
                {
                    sb.Append(c);
                }
            }
        }

        return sb.ToString();
    }

    private static string FinalizeHtml(string content, LuaTable config)
    {
        string html = string.Format("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"></head><body>{0}</body></html>", content);

        return StyleApplier.Apply(html, config);
    }
}
