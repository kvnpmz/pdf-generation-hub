using NLua;
using System.Text;

public class Inherit
{
    private readonly string _root = Paths.RootPath;

    public LuaTable Apply(string documentId, LuaTable config, Lua lua)
    {
        var extends = config["extends"]?.ToString();
        if (string.IsNullOrEmpty(extends))
            return config;

        var result = lua.DoString($"return require('documents.{extends}.config')");

        if (result == null || result.Length == 0)
        {
            Console.WriteLine($"[Inherit] ERROR: Could not find or load base config for '{extends}'");
            return config;
        }

        var baseConfig = (LuaTable)result[0];

        if (baseConfig == null)
        {
            Console.WriteLine($"[Inherit] ERROR: Loaded base config for '{extends}' but it was null.");
            return config;
        }

        var merged = Merge(baseConfig, config, lua);
        return merged;
    }


    private LuaTable LoadConfig(string documentId)
    {
        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;

        lua.DoString($"package.path = package.path .. ';{Path.Combine(_root, "?.tl")}'" +
                $" .. ';{Path.Combine(_root, "?/init.tl")}'"
                );

        lua.DoString("local tl = require('tl'); tl.loader();");

        return (LuaTable)lua.DoString($"return require('documents.{documentId}.config')")[0];
    }

    private LuaTable Merge(LuaTable baseConfig, LuaTable overrideConfig, Lua lua)
    {
        lua.DoString(@"
        function merge_tables(base, override)
            local result = {}

            for k,v in pairs(base) do
                result[k] = v
            end

            for k,v in pairs(override) do
                result[k] = v
            end

            return result
        end
    ");

        lua["base"] = baseConfig;
        lua["override"] = overrideConfig;

        var result = lua.DoString("return merge_tables(base, override)");

        return (LuaTable)result[0];
    }
}
