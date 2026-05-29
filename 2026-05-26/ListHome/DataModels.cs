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
    public float BaseHeaderHeight { get; set; } = 36.5f;
    public float BaseSubheaderHeight { get; set; } = 15f;
    public float BaseTitleHeight { get; set; } = 16.0f;
    public float BaseSubtitleHeight { get; set; } = 10.0f;
    public float BaseItemHeight { get; set; } = 15.8f;
    //public float BaseHeaderRulerHeight { get; set; } = 34f;

    public int MaxHeaderChars { get; set; } = 40;
    public int MaxSubheaderChars { get; set; } = 10;
    public int MaxTitleChars { get; set; } = 12;
    public int MaxSubtitleChars { get; set; } = 10;
    public int MaxItemChars { get; set; } = 15;

    public string FontFamily { get; set; } = "Inter";
    public float HeaderFontSize { get; set; } = 30f;
    public float SubheaderFontSize { get; set; } = 10f;
    public float TitleFontSize { get; set; } = 12.0f;
    public float SubtitleFontSize { get; set; } = 6.2f;
    public float ItemFontSize { get; set; } = 09.7f;
    public string PrimaryColor = "#121212";

    public float BulletPaddingTop { get; set; } = 0.0f;
    public float BulletSize { get; set; } = 9f;
    public float BulletThickness { get; set; } = 1f;
    public float ItemPaddingLeft { get; set; } = 12f;

    public float VerticalMargin = 36f;
    public float HorizontalMargin = 50.4f;
    public float ColumnSpacing = 8f;
    public string HeaderColor = "#3b6d6d";
    public string SubheaderColor = "#0000ff";
}
