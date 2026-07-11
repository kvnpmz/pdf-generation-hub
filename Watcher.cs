using TemplatePrintable.Core; 
using System.Threading.Channels;
using System.Text;
using NLua;

public class Watcher
{
    private readonly Flow _flow = new();
    private readonly string _root = Paths.RootPath;
    private readonly PluginLoader _loader = new();
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
            if (eventData.FullPath.Contains($"{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}"))
                return;

            if (IsTargetFile(eventData.Name))
                _eventChannel.Writer.TryWrite(eventData);
        };

        watcher.Changed += handler;
        watcher.Created += handler;
        watcher.Renamed += (sender, eventData) =>
        {
            if (eventData.FullPath.Contains($"{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}"))
                return;

            if (IsTargetFile(eventData.Name))
                _eventChannel.Writer.TryWrite(eventData);
        };
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
            string fileName = item.Name ?? "";

            await Task.Delay(250, cancellationToken);
            while (_eventChannel.Reader.TryRead(out _)) { }

            await BuildAsync(fileName);
        }
    }

    private async Task BuildAsync(string? changedFile = null)
    {
        if (changedFile != null && changedFile.EndsWith("render.cs"))
        {
            string? templateDir = Path.GetDirectoryName(changedFile);
            string? templateName = Path.GetFileName(templateDir);

            if (!string.IsNullOrEmpty(templateName))
            {
                LoadPlugin(templateName);
            }
        }

        using var lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;

        lua.DoString(
                $"package.path = package.path .. ';{Path.Combine(Paths.RootPath, "?.tl")}'" +
                $" .. ';{Path.Combine(Paths.RootPath, "?/init.tl")}'"
                );

        lua.DoString("local tl = require('tl'); tl.loader();");

        try
        {
            var (documentId, enableImages, baseProjectName) = LoadRuntime();

            var config = (LuaTable)lua.DoString(
                    $"return require('documents.{documentId}.config')"
                    )[0];

            var template = config["template"]?.ToString()
                ?? throw new Exception("Missing template");

            string renderCs = Path.Combine(Paths.RootPath, "templates", template, "render.cs");

            if (File.Exists(renderCs))
            {
                LoadPlugin(template);
            }

            var context = new Context
            {
                DocumentId = documentId,
                EnableImages = Convert.ToBoolean(enableImages),
                OutputDirectory = Path.Combine(Paths.RootPath, "output", documentId),
                BaseProjectName = baseProjectName,
                Flow = _flow
            };

            context.Config = config;

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

        return fileName.EndsWith(".tl") || fileName.EndsWith(".cs") || fileName.EndsWith(".css");
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

    private void RunCommand(string command, string args)
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(startInfo);
        process?.WaitForExit();
    }

    private void LoadPlugin(string templateName)
    {
        string outputDir = Path.Combine("plugins", templateName);
        string projectPath = Path.Combine("templates", templateName, $"{templateName}.csproj");

        DeleteDirectory(outputDir);

        RunCommand(
                "dotnet",
                $"build {projectPath} -o {outputDir} --nologo --verbosity quiet");

        var dllPath = Directory.GetFiles(outputDir, "grocery_list.dll").FirstOrDefault()
            ?? throw new Exception($"No plugin DLL found in {outputDir}");

        var pluginType = _loader.LoadPlugin(dllPath);

        if (pluginType != null)
        {
            var renderer = (IRenderer)Activator.CreateInstance(pluginType)!;
            _flow.RegisterRenderer(templateName, renderer);
        }
    }

    private void DeleteDirectory(string path)
    {
        for (int i = 0; i < 5; i++)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(500);
            }
        }
    }
}
