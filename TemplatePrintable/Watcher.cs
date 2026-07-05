using System.Threading.Channels;
using NLua;

public class Watcher
{
    private readonly Flow _flow = new();
    private readonly string _root = Paths.RootPath;
    private readonly Channel<FileSystemEventArgs> _eventChannel = Channel.CreateUnbounded<FileSystemEventArgs>();

    public async Task Start(CancellationToken ct = default)
    {
        _ = Task.Run(() => ProcessEventQueue(ct), ct);
        await BuildAsync();

        string directory = Path.GetDirectoryName(Path.GetFullPath(_root)) ?? _root;
        var watcher = new FileSystemWatcher(directory)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            InternalBufferSize = 65536
        };

        FileSystemEventHandler handler = (s, e) =>
        {
            if (IsTargetFile(e.Name))
            {
                _eventChannel.Writer.TryWrite(e);
            }
        };

        watcher.Changed += handler;
        watcher.Created += handler;
        watcher.Renamed += (s, e) => { if (IsTargetFile(e.Name)) _eventChannel.Writer.TryWrite(e); };
        watcher.Deleted += handler;

        watcher.EnableRaisingEvents = true;

        Console.WriteLine($"Watcher active on {directory}. Press Ctrl+C to exit.");

        try { await Task.Delay(Timeout.Infinite, ct); }
        catch (OperationCanceledException) { }
    }

    private async Task ProcessEventQueue(CancellationToken ct)
    {
        await foreach (var item in _eventChannel.Reader.ReadAllAsync(ct))
        {
            await Task.Delay(250, ct);
            
            while (_eventChannel.Reader.TryRead(out _)) { }

            await BuildAsync();
        }
    }

    private async Task BuildAsync()
    {
        try
        {
            var (documentId, enableImages, baseProjectName) = LoadRuntime();
            
            var context = new Context
            {
                DocumentId = documentId,
                EnableImages = Convert.ToBoolean(enableImages),
                OutputDirectory = Path.Combine("output", documentId),
                BaseProjectName = baseProjectName
            };

            await _flow.ExecuteAsync(context);
        }
        catch (NLua.Exceptions.LuaException ex)
        {
            Console.WriteLine($"LUA ERROR: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GENERAL ERROR: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private bool IsTargetFile(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        if (fileName.StartsWith("4913") || fileName.EndsWith("~") || fileName.StartsWith("."))
            return false;

        return fileName.EndsWith(".tl") || fileName.EndsWith(".css");
    }

    private (string documentId, int enableImages, string baseProjectName) LoadRuntime()
    {
        var lua = new Lua();
        var path = Path.Combine(Paths.RootPath, "runtime.tl");

        var result = lua.DoFile(path)[0] as LuaTable
            ?? throw new Exception("runtime.tl must return a table");

        var documentId = result["DOCUMENTID"]?.ToString()
            ?? throw new Exception("Missing DOCUMENTID");

        var baseProjectName = result["BASEPROJECTNAME"]?.ToString()
            ?? throw new Exception("Missing BASEPROJECTNAME");

        var enableImages = Convert.ToInt32(result["ENABLEIMAGES"] ?? 0);

        return (documentId, enableImages, baseProjectName);
    }
}
