public interface IPipelineStep
{
    Task ExecuteAsync(PipelineContext context);
}
