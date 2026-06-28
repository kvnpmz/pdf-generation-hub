public class Builder
{
    public async Task ExecuteAsync(string docId, int enableImages)
    {
        var context = new PipelineContext
        { 
            DocId = docId,
            EnableImages = Convert.ToBoolean(enableImages)
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
