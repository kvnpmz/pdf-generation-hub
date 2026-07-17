using System.Text;
using NLua;

public class LuaHost : IDisposable
{
    public Lua Lua { get; }

    public LuaHost()
    {
        Lua = new Lua();

        Lua.State.Encoding = Encoding.UTF8;

        ConfigurePackagePath();
        LoadTeal();
    }

    private void ConfigurePackagePath()
    {
        Lua.DoString(
            $"package.path = package.path .. ';{Path.Combine(Paths.RootPath, "?.tl")}'"
                + $" .. ';{Path.Combine(Paths.RootPath, "?/init.tl")}'"
        );
    }

    private void LoadTeal()
    {
        Lua.DoString("local tl = require('tl'); tl.loader();");
    }

    public void Dispose()
    {
        Lua.Dispose();
    }
}
