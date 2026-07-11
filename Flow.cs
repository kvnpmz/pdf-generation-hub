using TemplatePrintable.Core;

public class Flow
{
    private readonly Dictionary<string, IRenderer> _renderers = new();

    public void RegisterRenderer(string documentId, IRenderer renderer) 
    {
        _renderers[documentId] = renderer;
    }

    public bool TryGetRenderer(string documentId, out IRenderer? renderer) 
    {
        var found = _renderers.TryGetValue(documentId, out renderer);
        return found;
    }
    public async Task ExecuteAsync(Context context)
    {
        Directory.CreateDirectory(Path.Combine(Paths.RootPath, context.OutputDirectory));

        var steps = new List<IStep>
        {
            new Boot(),
            new Build(),
            new Render(),
            new Image(),
            new Indexer()
        };

        foreach (var step in steps)
        {
            await step.ExecuteAsync(context);
        }
    }
}
