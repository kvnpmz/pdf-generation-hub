public class Flow
{
    public async Task ExecuteAsync(string documentId, int enableImages, int isInteractive)
    {
        var context = new Context
        { 
            DocumentId = documentId,
            EnableImages = Convert.ToBoolean(enableImages),
            IsInteractive = Convert.ToBoolean(isInteractive),
            OutputDirectory = Path.Combine("output", documentId)
        };

        Directory.CreateDirectory(context.OutputDirectory);

        var steps = new List<IStep>
        {
            new Boot(),
            new Build(),
            new Render(),
            new Image()
        };

        foreach (var step in steps)
        {
            await step.ExecuteAsync(context);
        }
    }
}
