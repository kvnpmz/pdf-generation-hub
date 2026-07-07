using System.Threading.Channels;
using NLua;

public class Watcher
{
    private readonly Flow _flow = new();
    private readonly string _root = Paths.RootPath;
    private readonly Channel<FileSystemEventArgs> _eventChannel = Channel.CreateUnbounded<FileSystemEventArgs>();

    public async Task Start(CancellationToken cancellationToken = default)
    {
        _ = ProcessEventQueue(cancellationToken);
        await BuildAsync();

        string directory = Path.GetDirectoryName(Path.GetFullPath(_root)) ?? _root;
        using var watcher = new FileSystemWatcher(directory)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            InternalBufferSize = 65536
        };

        FileSystemEventHandler handler = (sender, eventData) =>
        {
            if (IsTargetFile(eventData.Name))
            {
                _eventChannel.Writer.TryWrite(eventData);
            }
        };

        watcher.Changed += handler;
        watcher.Created += handler;
        watcher.Renamed += (sender, eventData) => { if (IsTargetFile(eventData.Name)) _eventChannel.Writer.TryWrite(eventData); };
        watcher.Deleted += handler;

        watcher.EnableRaisingEvents = true;
        Console.WriteLine($"Watcher active on {directory}.");

        try { await Task.Delay(Timeout.Infinite, cancellationToken); }
        catch (OperationCanceledException) { }
    }

    private async Task ProcessEventQueue(CancellationToken cancellationToken)
    {
        await foreach (var item in _eventChannel.Reader.ReadAllAsync(cancellationToken))
        {
            await Task.Delay(250, cancellationToken);
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
        catch (NLua.Exceptions.LuaException exception)
        {
            Console.WriteLine($"LUA ERROR: {exception.Message}");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"GENERAL ERROR: {exception.GetType().Name}: {exception.Message}");
            Console.WriteLine(exception.StackTrace);
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

        lua.RegisterFunction("print", this, GetType().GetMethod(nameof(LuaPrint)));

        var result = lua.DoFile(path)[0] as LuaTable
            ?? throw new Exception("runtime.tl must return a table");

        var documentId = result["DOCUMENTID"]?.ToString()
            ?? throw new Exception("Missing DOCUMENTID");

        var baseProjectName = result["BASEPROJECTNAME"]?.ToString()
            ?? throw new Exception("Missing BASEPROJECTNAME");

        var enableImages = Convert.ToInt32(result["ENABLEIMAGES"] ?? 0);

        return (documentId, enableImages, baseProjectName);
    }

    public void LuaPrint(params object[] args)
    {
        Console.WriteLine("[LUA] " + string.Join(" ", args));
    }
}
