public interface IPdfRender
{
    Task ExecuteAsync(Context context);
}

public class Render : IStep
{
    public async Task ExecuteAsync(Context context)
    {
        IPdfRender pdfRender = context.IsEditable
            ? new Weasy()
            : new Chrome();

        await pdfRender.ExecuteAsync(context);
    }
}
