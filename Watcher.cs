using TemplatePrintable.Core; 
using System.Threading.Channels;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
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

        using var watcher = new FileSystemWatcher(_root)
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

            await BuildAsync(fullPath);
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
                $"package.path = package.path .. ';{Path.Combine(_root, "?.tl")}'" +
                $" .. ';{Path.Combine(_root, "?/init.tl")}'"
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

            string renderCs = Path.Combine(_root, "templates", template, "render.cs");

            if (File.Exists(renderCs))
            {
                LoadPlugin(template);
            }

            var context = new Context
            {
                DocumentId = documentId,
                EnableImages = Convert.ToBoolean(enableImages),
                OutputDirectory = Path.Combine(_root, "output", documentId),
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
        var path = Path.Combine(_root, "runtime.tl");

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
        string sourcePath = Path.Combine(_root, "templates", templateName, "render.cs");
        string sourceCode = File.ReadAllText(sourcePath);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IRenderer).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
            MetadataReference.CreateFromFile(Assembly.Load("NLua").Location)
        };

        var compilation = CSharpCompilation.Create($"{templateName}_{Guid.NewGuid()}",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            throw new Exception($"Compilation failed:\n{errors}");
        }

        ms.Position = 0;
        var pluginType = _loader.LoadPluginFromStream(ms);

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
