using TemplatePrintable.Core;
using System.Reflection;
using System.Runtime.Loader;

public class PluginLoader
{
    private AssemblyLoadContext? _context;

    public Type? LoadPlugin(string dllPath)
    {
        // 1. Unload the previous version
        if (_context != null)
        {
            _context.Unload();
            GC.Collect(); // Force collection to clear the old DLL from memory
            GC.WaitForPendingFinalizers();
        }

        // 2. Create a new context
        _context = new AssemblyLoadContext("PluginContext", isCollectible: true);
        
        // 3. Load the DLL
        var assembly = _context.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
        
        // 4. Return the first class that implements your Renderer interface
        return assembly.GetTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(IRenderer)));
    }
}
