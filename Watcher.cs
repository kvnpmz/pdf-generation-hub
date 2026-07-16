using System.Threading.Channels;

public class Watcher
{
    private readonly Flow _flow = new();
    private readonly Launcher _launcher;
    private readonly string _root = Paths.RootPath;
    private readonly Channel<FileSystemEventArgs> _eventChannel = Channel.CreateUnbounded<FileSystemEventArgs>();

    public static class BootState
    {
        public static bool SuppressEvents { get; set; }
    }

    public Watcher()
    {
        _launcher = new Launcher(_flow); 
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        await _launcher.BuildAsync();

        _ = ProcessEventQueue(cancellationToken);

        using var watcher = new FileSystemWatcher(_root)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            InternalBufferSize = 65536
        };

        FileSystemEventHandler handler = (sender, eventData) =>
        {
            if (BootState.SuppressEvents)
                return;

            if (IsTargetFile(eventData.Name))
                _eventChannel.Writer.TryWrite(eventData);
        };

        watcher.Changed += handler;
        watcher.Created += handler;
        watcher.Renamed += (sender, eventData) =>
        {
            if (BootState.SuppressEvents)
                return;

            if (IsTargetFile(eventData.Name))
                _eventChannel.Writer.TryWrite(eventData);
        };
        watcher.Deleted += handler;

        watcher.EnableRaisingEvents = true;
        Console.WriteLine($"Watcher active on {_root}.");

        try { await Task.Delay(Timeout.Infinite, cancellationToken); }
        catch (OperationCanceledException) { }
    }

    private async Task ProcessEventQueue(CancellationToken cancellationToken)
    {
        await foreach (var item in _eventChannel.Reader.ReadAllAsync(cancellationToken))
        {
            string fullPath = item.FullPath;

            await Task.Delay(250, cancellationToken);
            while (_eventChannel.Reader.TryRead(out _)) { }

            await _launcher.BuildAsync(fullPath);
        }
    }

    private bool IsTargetFile(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        if (fileName.StartsWith("4913") || fileName.EndsWith("~") || fileName.StartsWith("."))
            return false;

        return fileName.EndsWith(".tl") || fileName.EndsWith(".cs") || fileName.EndsWith(".css");
    }
}
