using NLua;

public interface IRenderer
{
    RenderResult Render(LuaTable config);
}

public interface IStep
{
    Task ExecuteAsync(Context context);
}

public class Flow
{
    private readonly Dictionary<string, IRenderer> _renderers = new();

    public void RegisterRenderer(string templateName, IRenderer renderer)
    {
        _renderers[templateName] = renderer;
    }

    public bool TryGetRenderer(string templateName, out IRenderer? renderer)
    {
        var found = _renderers.TryGetValue(templateName, out renderer);
        return found;
    }

    public async Task ExecuteAsync(Context context)
    {
        Directory.CreateDirectory(context.OutputDirectory);

        var steps = new List<IStep>
        {
            new Build(),
            new Export(),
            new Image(),
            new Indexer()
        };

        foreach (var step in steps)
        {
            await step.ExecuteAsync(context);
        }
    }
}
