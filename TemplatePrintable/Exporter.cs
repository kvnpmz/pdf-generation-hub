public interface IPdfExport
{
    Task ExecuteAsync(PipelineContext context);
}

public class Exporter : IPipelineStep
{
    public async Task ExecuteAsync(PipelineContext context)
    {
        IPdfExport pdfExport = context.IsInteractive
            ? new WeasyPrintExport()
            : new PlaywrightExport();

        await pdfExport.ExecuteAsync(context);
    }
}
