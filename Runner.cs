using TemplatePrintable.Core; 
using NLua;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

public class Runner
{
    private readonly Flow _flow;
    private readonly string _root = Paths.RootPath;
    private readonly Resolver _resolver = new();

    public Runner(Flow flow)
    {
        _flow = flow;
    }

    public async Task BuildAsync(string? changedFile = null)
    {
        if (changedFile != null && changedFile.EndsWith("render.cs"))
        {
            string? templateDirectory = Path.GetDirectoryName(changedFile);
            string? templateName = Path.GetFileName(templateDirectory);

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

        using var memoryStream = new MemoryStream();
        var result = compilation.Emit(memoryStream);

        if (!result.Success)
        {
            var errorList = new List<Diagnostic>();

            foreach (var d in result.Diagnostics)
            {
                if (d.Severity == DiagnosticSeverity.Error)
                {
                    errorList.Add(d);
                }
            }

            var errors = string.Join("\n", errorList);

            throw new Exception($"Compilation failed:\n{errors}");
        }

        memoryStream.Position = 0;
        var pluginType = _resolver.LoadPluginFromStream(memoryStream);

        if (pluginType != null)
        {
            var renderer = (IRenderer)Activator.CreateInstance(pluginType)!;
            _flow.RegisterRenderer(templateName, renderer);
        }
    }

    public void LuaPrint(params object[] args)
    {
        Console.WriteLine("[LUA] " + string.Join(" ", args));
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
}
