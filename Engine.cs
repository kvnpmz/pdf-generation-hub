public interface IPdfEngine
{
    Task ExecuteAsync(Context context);
}

public class Engine : IStep
{
    public async Task ExecuteAsync(Context context)
    {
        IPdfEngine pdfEngine = context.IsEditable
            ? new Weasy()
            : new Chrome();

        await pdfEngine.ExecuteAsync(context);
    }
}
