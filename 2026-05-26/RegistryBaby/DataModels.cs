using System.Collections.Generic;

public class DataSection
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public List<string> Items { get; set; } = new();
}
public class DataColumn
{
    public List<DataSection> Sections { get; set; } = new();
}
public class ReportTheme
{
    public string FontFamily { get; set; } = "Liberation Sans";
    public float HeaderFontSize { get; set; } = 30f;
    public float SubheaderFontSize { get; set; } = 10f;
    public float TitleFontSize { get; set; } = 13.0f;
    public float SubtitleFontSize { get; set; } = 6.2f;
    public float ItemFontSize { get; set; } = 11.7f;
    public string PrimaryColor = "#121212";

    public float BulletPaddingTop { get; set; } = 0.5f;
    public float BulletSize { get; set; } = 6f;
    public float BulletThickness { get; set; } = 1f;

    public float BaseHeaderHeight { get; set; } = 36f;
    public float BaseSubheaderHeight { get; set; } = 15f;
    public float BaseTitleHeight { get; set; } = 21.0f;
    public float BaseSubtitleHeight { get; set; } = 10.0f;
    public float BaseItemHeight { get; set; } = 18.6f;

    public int MaxHeaderChars { get; set; } = 20;
    public int MaxSubheaderChars { get; set; } = 10;
    public int MaxTitleChars { get; set; } = 20;
    public int MaxSubtitleChars { get; set; } = 10;
    public int MaxItemChars { get; set; } = 27;
}
