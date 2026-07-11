public static class DocumentGenerator
{
    public static string GenerateDocument(Dictionary<string, object> data)
    {
        var renderTemplate = (Func<object, string>)data["renderTemplate"];
        var config = data["config"];

        string html = renderTemplate(config);

        html = string.Format(
                "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"></head><body>{0}</body></html>",
                html
                );

        return html;
    }
}
