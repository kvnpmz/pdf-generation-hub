using TemplatePrintable.Core;
using System.Reflection;
using System.Runtime.Loader;

public class PluginLoader
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

        // Load assembly from stream
        var assembly = _context.LoadFromStream(stream);

        return assembly.GetTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(IRenderer)));
    }
}
