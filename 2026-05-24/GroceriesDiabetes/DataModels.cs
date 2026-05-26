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

    public float H2PaddingBottom { get; set; } = 2f;
    public float BulletPaddingTop { get; set; } = 1f;
    public float BulletSize { get; set; } = 5f;

    public float BaseH1Height { get; set; } = 36f;
    public float BaseH2Height { get; set; } = 10.0f;
    public float BaseItemHeight { get; set; } = 13.5f;

    public int MaxTitleChars { get; set; } = 20;
    public int MaxItemChars { get; set; } = 14;
}
