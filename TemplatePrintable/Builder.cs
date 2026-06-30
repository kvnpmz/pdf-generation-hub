public class Builder
{
    public async Task ExecuteAsync(string documentId, int enableImages, int isInteractive)
    {
        var context = new PipelineContext
        { 
            DocumentId = documentId,
            EnableImages = Convert.ToBoolean(enableImages),
            IsInteractive = Convert.ToBoolean(isInteractive),
            OutputDirectory = Path.Combine("output", documentId)
        };

        Directory.CreateDirectory(context.OutputDirectory);

        var steps = new List<IPipelineStep>
        {
            new Initializer(),
            new Renderer(),
            new Exporter(),
            new ImageConverter()
        };

        foreach (var step in steps)
        {
            await step.ExecuteAsync(context);
        }
    }
}
