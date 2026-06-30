var builder = new Builder();
await builder.ExecuteAsync("weekly_schedule", 0, 0);

var html = await File.ReadAllTextAsync("preview.html");
await WeasyPrint.RenderAsync(html, "editable");
