public class Builder
{
    public async Task ExecuteAsync(string documentId, int enableImages, int isInteractive)
    {
        var context = new PipelineContext
        { 
            DocumentId = documentId,
            EnableImages = Convert.ToBoolean(enableImages),
            IsInteractive = Convert.ToBoolean(isInteractive)
        };

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
