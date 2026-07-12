using TemplatePrintable.Core;
using System.Reflection;
using System.Runtime.Loader;

public class Resolver
{
    private AssemblyLoadContext? _context;

    public Type? LoadPluginFromStream(Stream stream)
    {
        if (_context != null)
        {
            _context.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        _context = new AssemblyLoadContext("PluginContext", isCollectible: true);

        var assembly = _context.LoadFromStream(stream);

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAssignableTo(typeof(IRenderer)))
            {
                return type;
            }
        }

        return null;
    }
}
