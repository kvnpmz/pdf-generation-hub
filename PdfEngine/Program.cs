string id = "apartment_checklist_grid";
var copier = new ProjectInitializer();

copier.CreateFolder(id);
var engine = new DocumentEngine();

var result = await engine.GenerateHtml(id);
string html = result.html;

string outputName = result.outputName;
var converter = new GeneratePdf();

await converter.ConvertToPdfAsync(html, id, outputName);
var processor = new ImageProcessor();

processor.IsEnabled = Convert.ToBoolean(1); 
processor.ProcessFolder(id);
