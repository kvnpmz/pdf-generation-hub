string id = "new_checklist_2";
var copier = new ProjectInitializer();
copier.CreateFolder(id);

var engine = new DocumentEngine();

string html = await engine.GenerateHtml(id);
var converter = new GeneratePdf();

await converter.ConvertToPdfAsync(html, id);
var processor = new ImageProcessor();

processor.IsEnabled = Convert.ToBoolean(1); 
processor.ProcessFolder(id);
