var docName = "apartment_checklist";
var copier = new DocumentBoilerplate();
copier.CloneDocument("example_checklist", docName);

var engine = new DocumentEngine();
var converter = new GeneratePdf();

string html = await engine.GenerateHtml(docName);
await converter.ConvertToPdfAsync(html, docName);

string folderPath = Path.Combine(Environment.CurrentDirectory, "documents", docName);
string finalPdfName = $"{docName}_letter.pdf";
string fullPdfPath = Path.Combine(folderPath, finalPdfName);

var processor = new ImageProcessor();
processor.IsEnabled = true;
processor.RunPipeline(fullPdfPath);
