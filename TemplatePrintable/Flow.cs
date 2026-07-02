public class Flow
{
    public async Task ExecuteAsync(string documentId, int enableImages)
    {
        var context = new Context
        { 
            DocumentId = documentId,
            EnableImages = Convert.ToBoolean(enableImages),
            OutputDirectory = Path.Combine("output", documentId)
        };

        Directory.CreateDirectory(context.OutputDirectory);

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
