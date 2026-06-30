public interface IPdfRender
{
    Task ExecuteAsync(Context context);
}

public class Render : IStep
{
    public async Task ExecuteAsync(Context context)
    {
        IPdfRender pdfRender = context.IsInteractive
            ? new Weasy()
            : new Chrome();

        await pdfRender.ExecuteAsync(context);
    }
}
