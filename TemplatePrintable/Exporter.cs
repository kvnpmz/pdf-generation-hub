public class Exporter : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        IPipelineStep exporter = context.IsInteractive 
            ? new WeasyPrintExporter() 
            : new PlaywrightExporter();

        await exporter.ExecuteAsync(context);
    }
}
