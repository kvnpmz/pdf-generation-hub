public class Flow
{
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
