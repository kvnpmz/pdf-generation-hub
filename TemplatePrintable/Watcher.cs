public class Watcher 
{
    private readonly string _documentId;
    private readonly int _enableImages;
    private readonly Flow _flow = new();
    private bool _isBuilding, _rebuildRequested;
    private CancellationTokenSource? _debounce;
    private readonly string _root = AppConfig.RootPath;

    public Watcher(string documentId, int enableImages)
    {
        _documentId = documentId;
        _enableImages = enableImages;
    }

    public async Task Start()
    {
        await BuildAsync();

        var watcher = new FileSystemWatcher(_root)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            EnableRaisingEvents = true
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;
        watcher.Renamed += OnChanged;
        watcher.Deleted += OnChanged;

        await Task.Delay(Timeout.Infinite);
    }

    private async Task BuildAsync()
    {
        if (_isBuilding)
        {
            _rebuildRequested = true;
            return;
        }

        _isBuilding = true;

        try
        {
            do
            {
                _rebuildRequested = false;

                Console.WriteLine($"[{DateTime.Now:T}] Building...");

                await _flow.ExecuteAsync(_documentId, _enableImages);
            }
            while (_rebuildRequested);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _isBuilding = false;
        }
    }

    private void OnChanged(object? sender, FileSystemEventArgs e)
    {
        var ext = Path.GetExtension(e.FullPath);
        if (ext != ".tl" && ext != ".css") return;

        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            await Task.Delay(250, _debounce.Token);
            await BuildAsync();
        });
    }
}
