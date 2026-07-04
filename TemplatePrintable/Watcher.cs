public class Watcher 
{
    private readonly string _documentId;
    private readonly int _enableImages;
    private readonly Flow _flow = new();
    private bool _isBuilding, _rebuildRequested;
    private CancellationTokenSource? _debounce;
    private readonly string _root = Paths.RootPath;
    private readonly string _baseProjectName;

    public Watcher(string documentId, int enableImages, string baseProjectName)
    {
        _documentId = documentId;
        _enableImages = enableImages;
        _baseProjectName = baseProjectName;
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

                var context = new Context
                {
                    DocumentId = _documentId,
                    EnableImages = Convert.ToBoolean(_enableImages),
                    OutputDirectory = Path.Combine("output", _documentId),
                    BaseProjectName = _baseProjectName
                };

                await _flow.ExecuteAsync(context);
            }
            while (_rebuildRequested);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
        finally
        {
            _isBuilding = false;
        }
    }

    private void OnChanged(object? sender, FileSystemEventArgs eventArgs)
    {
        var extension = Path.GetExtension(eventArgs.FullPath);
        if (extension != ".tl" && extension != ".css") return;

        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            await Task.Delay(250, _debounce.Token);
            await BuildAsync();
        });
    }
}
