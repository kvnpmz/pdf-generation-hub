public class Builder
{
    public async Task ExecuteAsync(string docId, int imageToggle)
    {
        var context = new PipelineContext { DocId = docId };

        var steps = new List<IPipelineStep>
        {
            new Initializer(),
            new Renderer(),
            new Exporter()
        };

        if (Convert.ToBoolean(imageToggle))
        {
            steps.Add(new ImageConverter());
        }
        else
        {
            Console.WriteLine("[INFO] ImageProcessor is disabled. Skipping.");
        }

        foreach (var step in steps)
        {
            await step.ExecuteAsync(context);
        }
    }
}
