using System.Diagnostics;
using System.Text;
using NLua;

public class TealRenderProvider : IRenderProvider
{
    public bool CanRender(string template)
    {
        return File.Exists(Path.Combine(Paths.RootPath, "templates", template, "render.tl"));
    }

    public async Task<RenderResult> RenderAsync(Context context, LuaTable config)
    {
        using var lua = new Lua();

        lua.State.Encoding = Encoding.UTF8;

        var luaPath =
            $"package.path = package.path .. ';{Path.Combine(Paths.RootPath, "?.tl")}' " +
            $".. ';{Path.Combine(Paths.RootPath, "?/init.tl")}'";

        lua.DoString(luaPath);

        lua["ROOT_PATH"] = Paths.RootPath;

        lua.DoString("local tl = require('tl'); tl.loader();");

        var template = config["template"]?.ToString() ?? throw new Exception("Missing template");

        await RunTlCheckAsync(new[]
                { 
                "init.tl",
                Path.Combine("documents", context.DocumentId, "config.tl"),
                Path.Combine("templates", template, "render.tl")
                });

        var exports = (LuaTable)lua.DoString("return require('init')")[0];
        var render = (LuaFunction)exports["Render"];
        var result = (LuaTable)render.Call(context.DocumentId)[0];

        return new RenderResult
        {
            Html = result["html"]?.ToString() ?? "",
            OutputName = result["outputName"]?.ToString() ?? "output"
        };
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
}
