using NLua;

public class PluginRenderProvider : IRenderProvider
{
    public bool CanRender(string template)
    {
        return File.Exists(Path.Combine(Paths.RootPath, "templates", template, "render.cs"));
    }

    public Task<RenderResult> RenderAsync(Context context, LuaTable config)
    {
        var template = config["template"]?.ToString() ?? throw new Exception("Missing template");

        if (context.Flow == null || !context.Flow.TryGetRenderer(template, out var renderer))
        {
            throw new Exception($"No renderer registered for '{template}'");
        }

        return Task.FromResult(renderer!.Render(config));
    }
}
