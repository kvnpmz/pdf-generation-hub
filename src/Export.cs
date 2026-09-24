public interface IPdfExport
{
    Task ExecuteAsync(Context context);
}

public class Export : IStep
{
    public async Task ExecuteAsync(Context context)
    {
        IPdfExport pdfExport = context.IsEditable ? new Weasy() : new Chrome();

        await pdfExport.ExecuteAsync(context);
    }
}
