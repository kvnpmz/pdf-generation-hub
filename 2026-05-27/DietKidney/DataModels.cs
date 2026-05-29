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
    public float BaseSubheaderHeight { get; set; } = 25f;
    public float BaseTitleHeight { get; set; } = 12.0f;
    public float BaseSubtitleHeight { get; set; } = 10.0f;
    public float BaseItemHeight { get; set; } = 15.9f;
    //public float BaseHeaderRulerHeight { get; set; } = 34f;

    public int MaxHeaderChars { get; set; } = 40;
    public int MaxSubheaderChars { get; set; } = 10;
    public int MaxTitleChars { get; set; } = 35;
    public int MaxSubtitleChars { get; set; } = 30;
    public int MaxItemChars { get; set; } = 42;

    public string FontFamily { get; set; } = "Inter";
    public float HeaderFontSize { get; set; } = 28.0f;
    public float SubheaderFontSize { get; set; } = 16.5f;
    public float TitleFontSize { get; set; } = 08.0f;
    public float SubtitleFontSize { get; set; } = 6.2f;
    public float ItemFontSize { get; set; } = 10.0f;
    public string PrimaryColor = "#121212";

    public float BulletPaddingTop { get; set; } = 1.0f;
    public float BulletSize { get; set; } = 3f;
    public float BulletThickness { get; set; } = 1f;
    public float ItemPaddingLeft { get; set; } = 6f;

    public float VerticalMargin = 36f;
    public float HorizontalMargin = 50.4f;
    public float ColumnSpacing = 5f;
    public float ColumnPadding = 3f;
    public string HeaderColor = "#3b6d6d";
    public string SubheaderColor = "#0000ff";
    public float HeaderLetterSpacing = 0.000f;
}
