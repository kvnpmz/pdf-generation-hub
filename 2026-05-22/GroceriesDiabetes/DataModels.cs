using System.Collections.Generic;

public class DataSection
{
    public string Title { get; set; } = "";
    public List<string> Items { get; set; } = new();
}
public class DataColumn
{
    public List<DataSection> Sections { get; set; } = new();
}
public class ReportTheme
{
    public string FontFamily { get; set; } = "Liberation Sans";
    public float TitleFontSize { get; set; } = 30f;
    public float HeaderTableFontSize { get; set; } = 16f;
    public float SectionTitleFontSize { get; set; } = 6.2f;
    public float ItemFontSize { get; set; } = 7.0f;
    public string PrimaryColor = "#121212";
}
