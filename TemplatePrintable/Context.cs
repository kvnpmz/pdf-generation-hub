public class Context
{
    public string DocumentId { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;

    public string OutputName { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = string.Empty;

    public bool EnableImages { get; set; }
    public bool IsInteractive { get; set; }
}
