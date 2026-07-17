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
        try
        {
            using var lua = new Lua();
            lua.State.Encoding = Encoding.UTF8;
            var luaPath = $"package.path = package.path .. ';{Path.Combine(Paths.RootPath, "?.tl")}' " +
                $".. ';{Path.Combine(Paths.RootPath, "?/init.tl")}'";

            lua.DoString(luaPath);
            lua["ROOT_PATH"] = Paths.RootPath;
            lua.DoString("local tl = require('tl'); tl.loader();");

            await RunTlCheckAsync(new[]
                    {
                    "init.tl",
                    Path.Combine("documents", context.DocumentId, "config.tl"),
                    Path.Combine("templates", config["template"]?.ToString() ?? "", "render.tl")
                    });

            lua["__documentId"] = context.DocumentId;
            lua.DoString("config = {}");

            var luaConfig = (LuaTable)lua["config"];
            CopyTableToLua(lua, config, luaConfig);
            lua["__config"] = luaConfig;

            var values = lua.DoString(@"
                    local init = require('init')
                    local ok, result = pcall(init.Render, __config)

                    if not ok then
                    error(result)
                    end

                    return result
                    ");

            if (values == null || values.Length == 0) throw new Exception("Lua returned nothing");

            var resultTable = values[0] as LuaTable ?? throw new Exception("Lua returned non-table");

            return new RenderResult
            {
                Html = resultTable["html"]?.ToString() ?? "",
                OutputName = resultTable["outputName"]?.ToString() ?? "output"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("INIT/RENDER FAILED:");
            Console.WriteLine(ex);
            throw;
        }
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

    private static void CopyTableToLua(Lua lua, LuaTable source, LuaTable destination)
    {
        foreach (var key in source.Keys)
        {
            var value = source[key];

            if (value is LuaTable child)
            {
                lua.DoString("__tmp = {}");
                var childTable = (LuaTable)lua["__tmp"];

                CopyTableToLua(lua, child, childTable);

                destination[key] = childTable;
            }
            else
            {
                destination[key] = value;
            }
        }
    }
}
