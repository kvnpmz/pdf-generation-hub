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

        var baseConfig = (LuaTable)lua.DoString($"return require('documents.{extends}.config')")[0];

        return Merge(baseConfig, config);
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


    private LuaTable Merge(LuaTable baseConfig, LuaTable overrideConfig)
    {
        string[] keysToInherit = 
        {
            "output_name",
            "template",
            "header",
            "columns",
            "sections",
            "items",
            "layout"
        };

        foreach (var key in keysToInherit)
        {
            if (overrideConfig[key] == null && baseConfig[key] != null)
            {
                overrideConfig[key] = baseConfig[key];
            }
        }

        return overrideConfig;
    }
}
