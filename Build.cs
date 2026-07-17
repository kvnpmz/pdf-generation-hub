using System.Diagnostics;
using System.Text;
using NLua;

public class RenderResult
{
    public string Html { get; set; } = "";
    public string OutputName { get; set; } = "output";
}

public interface IRenderProvider
{
    bool CanRender(string template);

    Task<RenderResult> RenderAsync(Context context, LuaTable config);
}

public class Build : IStep
{
    private readonly List<IRenderProvider> _providers;

    public Build()
    {
        _providers = new() { new TealRenderProvider(), new PluginRenderProvider() };
    }

    public async Task ExecuteAsync(Context context)
    {
        var config = context.Config ?? throw new Exception("Context.Config is null");

        var template = config["template"]?.ToString() ?? throw new Exception("Missing template");

        context.IsEditable = config["is_editable"] is bool b && b;

        var renderPath = Path.Combine(Paths.RootPath, "templates", template, "render");

        IRenderProvider? provider = null;

        foreach (var item in _providers)
        {
            if (item.CanRender(template))
            {
                provider = item;
                break;
            }
        }

        if (provider == null)
        {
            throw new Exception($"No renderer found for '{template}'");
        }

        var result = await provider.RenderAsync(context, config);
        context.Html = FinalizeHtml(result.Html, config);

        context.OutputName = result.OutputName;
        string formattedHtml = Format.Beautify(context.Html);

        var htmlPath = Path.Combine(context.OutputDirectory, $"{context.OutputName}.html");
        var previewPath = Path.Combine(Paths.RootPath, "preview.html");

        File.WriteAllText(htmlPath, formattedHtml);
        File.WriteAllText(previewPath, formattedHtml);
    }

    private static string FinalizeHtml(string content, LuaTable config)
    {
        string html = string.Format(
            "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"></head><body>{0}</body></html>",
            content
        );

        return StyleApplier.Apply(html, config);
    }
}
